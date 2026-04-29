-- إضافة التصنيفات فقط إذا لم تكن موجودة
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Electronics')
    INSERT INTO Categories (Name, Icon) VALUES ('Electronics', 'bi-laptop');
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Furniture')
    INSERT INTO Categories (Name, Icon) VALUES ('Furniture', 'bi-house-door');
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Tools')
    INSERT INTO Categories (Name, Icon) VALUES ('Tools', 'bi-wrench');
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Cameras & Photo')
    INSERT INTO Categories (Name, Icon) VALUES ('Cameras & Photo', 'bi-camera');
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Sports Equipment')
    INSERT INTO Categories (Name, Icon) VALUES ('Sports Equipment', 'bi-bicycle');
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Books & Media')
    INSERT INTO Categories (Name, Icon) VALUES ('Books & Media', 'bi-book');
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Party & Event Supplies')
    INSERT INTO Categories (Name, Icon) VALUES ('Party & Event Supplies', 'bi-balloon');
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Vehicles')
    INSERT INTO Categories (Name, Icon) VALUES ('Vehicles', 'bi-car-front');
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Clothing & Accessories')
    INSERT INTO Categories (Name, Icon) VALUES ('Clothing & Accessories', 'bi-bag');
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Gardening')
    INSERT INTO Categories (Name, Icon) VALUES ('Gardening', 'bi-flower1');
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Musical Instruments')
    INSERT INTO Categories (Name, Icon) VALUES ('Musical Instruments', 'bi-music-note-beamed');
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Handmade & Crafts')
    INSERT INTO Categories (Name, Icon) VALUES ('Handmade & Crafts', 'bi-scissors');
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Office Equipment')
    INSERT INTO Categories (Name, Icon) VALUES ('Office Equipment', 'bi-printer');
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Pet Supplies')
    INSERT INTO Categories (Name, Icon) VALUES ('Pet Supplies', 'bi-heart');
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Services')
    INSERT INTO Categories (Name, Icon) VALUES ('Services', 'bi-person-video');
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Outdoor & Camping')
    INSERT INTO Categories (Name, Icon) VALUES ('Outdoor & Camping', 'bi-tree');
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Health & Beauty')
    INSERT INTO Categories (Name, Icon) VALUES ('Health & Beauty', 'bi-heart-pulse');
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Baby & Kids')
    INSERT INTO Categories (Name, Icon) VALUES ('Baby & Kids', 'bi-baby');
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Education & Tutoring')
    INSERT INTO Categories (Name, Icon) VALUES ('Education & Tutoring', 'bi-book-half');
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Photography Services')
    INSERT INTO Categories (Name, Icon) VALUES ('Photography Services', 'bi-camera-reels');