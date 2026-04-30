using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SW_Project.Data;
using SW_Project.Models;
using SW_Project.ViewModels.Message;

namespace SW_Project.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MessagesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Inbox - عرض كافة المحادثات
        public async Task<IActionResult> Inbox()
        {
            var userId = _userManager.GetUserId(User);

            var conversations = await _context.Conversations
                .Include(c => c.ParticipantA)
                .Include(c => c.ParticipantB)
                .Include(c => c.Listing)
                    .ThenInclude(l => l.ListingImages)
                .Where(c => c.ParticipantAId == userId || c.ParticipantBId == userId)
                .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
                .ToListAsync();

            var inboxItems = new List<InboxItemVM>();

            foreach (var conv in conversations)
            {
                var otherUser = conv.ParticipantAId == userId ? conv.ParticipantB : conv.ParticipantA;

                var lastMessage = await _context.Messages
                    .Where(m => m.ConversationId == conv.Id)
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => new { m.Text, m.SentAt })
                    .FirstOrDefaultAsync();

                var unreadCount = await _context.Messages
                    .CountAsync(m => m.ConversationId == conv.Id && m.ReceiverId == userId && !m.IsRead);

                inboxItems.Add(new InboxItemVM
                {
                    ConversationId = conv.Id,
                    OtherUserId = otherUser.Id,
                    OtherUserName = otherUser.Name,
                    OtherUserAvatar = !string.IsNullOrEmpty(otherUser.Name) ? otherUser.Name.Substring(0, 1).ToUpper() : "?",
                    LastMessage = lastMessage?.Text ?? "No messages yet",
                    LastMessageAt = lastMessage?.SentAt ?? conv.CreatedAt,
                    UnreadCount = unreadCount,
                    ListingId = conv.ListingId,
                    ListingTitle = conv.Listing?.Title,
                    ListingImageUrl = conv.Listing?.ListingImages?.FirstOrDefault()?.ImagePath
                });
            }

            var viewModel = new InboxVM
            {
                Conversations = inboxItems.OrderByDescending(i => i.LastMessageAt).ToList(),
                UnreadCount = inboxItems.Sum(i => i.UnreadCount)
            };

            return View(viewModel);
        }

        // GET: Conversation - عرض محادثة محددة
        [HttpGet]
        public async Task<IActionResult> Conversation(string userId, int? listingId = null)
        {
            try
            {
                var currentUserId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(userId))
                    return RedirectToAction("Inbox");

                if (userId == currentUserId) return RedirectToAction("Inbox");

                var otherUser = await _userManager.FindByIdAsync(userId);
                if (otherUser == null) return RedirectToAction("Inbox");

                var conversation = await _context.Conversations
                    .Include(c => c.Messages)
                        .ThenInclude(m => m.Sender)
                    .Include(c => c.Listing)
                        .ThenInclude(l => l.ListingImages)
                    .FirstOrDefaultAsync(c =>
                        (c.ParticipantAId == currentUserId && c.ParticipantBId == userId) ||
                        (c.ParticipantAId == userId && c.ParticipantBId == currentUserId));

                if (conversation == null)
                {
                    conversation = new Conversation
                    {
                        ParticipantAId = currentUserId,
                        ParticipantBId = userId,
                        ListingId = listingId,
                        CreatedAt = DateTime.Now,
                        LastMessageAt = DateTime.Now // تجنب الـ NULL من البداية
                    };
                    _context.Conversations.Add(conversation);
                    await _context.SaveChangesAsync();
                }

                // تحديث الرسائل لتصبح "مقروءة"
                var unreadMessages = _context.Messages.Where(m => m.ConversationId == conversation.Id && m.ReceiverId == currentUserId && !m.IsRead);
                foreach (var msg in unreadMessages)
                {
                    msg.IsRead = true;
                    msg.ReadAt = DateTime.Now;
                }
                await _context.SaveChangesAsync();

                var messages = conversation.Messages?.OrderBy(m => m.SentAt).Select(m => new MessageItemVM
                {
                    Id = m.Id,
                    Text = m.Text,
                    SenderId = m.SenderId,
                    SenderName = m.Sender?.Name ?? "User",
                    IsFromCurrentUser = m.SenderId == currentUserId,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead
                }).ToList() ?? new List<MessageItemVM>();

                var viewModel = new ConversationVM
                {
                    ConversationId = conversation.Id,
                    OtherUserId = otherUser.Id,
                    OtherUserName = otherUser.Name,
                    OtherUserAvatar = !string.IsNullOrEmpty(otherUser.Name) ? otherUser.Name.Substring(0, 1).ToUpper() : "?",
                    ListingId = conversation.ListingId,
                    ListingTitle = conversation.Listing?.Title,
                    ListingImageUrl = conversation.Listing?.ListingImages?.FirstOrDefault()?.ImagePath,
                    Messages = messages,
                    SendMessage = new SendMessageVM
                    {
                        ReceiverId = userId,
                        ListingId = conversation.ListingId
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                return RedirectToAction("Inbox");
            }
        }

        // POST: SendMessage - إرسال الرسالة وحفظها
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(SendMessageVM model)
        {
            System.Diagnostics.Debug.WriteLine($"DEBUG: Received Text: '{model.Text}'");
            System.Diagnostics.Debug.WriteLine($"DEBUG: Received ReceiverId: {model.ReceiverId}");
            if (string.IsNullOrWhiteSpace(model.Text))
                return RedirectToAction("Conversation", new { userId = model.ReceiverId, listingId = model.ListingId });

            try
            {
                var currentUserId = _userManager.GetUserId(User);

                // 1. التأكد من وجود المحادثة
                var conversation = await _context.Conversations
                    .FirstOrDefaultAsync(c =>
                        (c.ParticipantAId == currentUserId && c.ParticipantBId == model.ReceiverId) ||
                        (c.ParticipantAId == model.ReceiverId && c.ParticipantBId == currentUserId));

                if (conversation == null)
                {
                    conversation = new Conversation
                    {
                        ParticipantAId = currentUserId,
                        ParticipantBId = model.ReceiverId,
                        ListingId = model.ListingId,
                        CreatedAt = DateTime.Now
                    };
                    _context.Conversations.Add(conversation);

                    await _context.SaveChangesAsync();
                }

                // 2. إنشاء الرسالة وحفظها (هذا ما سيجعل جدول Messages ممتلئاً)
                var message = new Message
                {
                    ConversationId = conversation.Id,
                    SenderId = currentUserId,
                    ReceiverId = model.ReceiverId,
                    Text = model.Text,
                    SentAt = DateTime.Now,
                    IsRead = false
                };

                _context.Messages.Add(message);
                conversation.LastMessageAt = message.SentAt;

                // 2. فحص قبل الحفظ مباشرة
                System.Diagnostics.Debug.WriteLine("DEBUG: Attempting to save to database...");
                var result = await _context.SaveChangesAsync();
                System.Diagnostics.Debug.WriteLine($"DEBUG: Save changes returned: {result} rows affected");

                return RedirectToAction("Conversation", new { userId = model.ReceiverId, listingId = model.ListingId });
            }
            catch (Exception ex)
            {
                // 3. طباعة الخطأ التفصيلي لو الداتابيز رفضت
                System.Diagnostics.Debug.WriteLine($"DEBUG: CRITICAL ERROR: {ex.Message}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"DEBUG: Inner Exception: {ex.InnerException.Message}");

                TempData["Error"] = "Failed to send.";
                return RedirectToAction("Inbox");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Json(new { count = 0 });

            var unreadCount = await _context.Messages
                .CountAsync(m => m.ReceiverId == userId && !m.IsRead);

            return Json(new { count = unreadCount });
        }

        [HttpPost]
        public async Task<IActionResult> StartConversation(int listingId)
        {
            var listing = await _context.Listings.FindAsync(listingId);
            if (listing == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (listing.OwnerId == currentUserId) return RedirectToAction("Details", "Listings", new { id = listingId });

            return RedirectToAction("Conversation", new { userId = listing.OwnerId, listingId = listingId });
        }
    }
}