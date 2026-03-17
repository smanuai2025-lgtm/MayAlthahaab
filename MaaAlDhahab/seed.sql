-- ═══════════════════════════════════════════════════════════════
-- ماء الذهب | Maa Al Dhahab Perfumes - SQL Seed Script
-- SQL Server - Run AFTER database migration (dotnet ef database update)
-- Or use EF Core automatic seeding via DbSeeder.cs (recommended)
-- ═══════════════════════════════════════════════════════════════

USE MaaAlDhahabDB;
GO

-- ── Clear existing data (for re-seeding) ──
-- DELETE FROM OrderItems; DELETE FROM Orders; DELETE FROM WishlistItems;
-- DELETE FROM Reviews; DELETE FROM Products; DELETE FROM Categories;
-- DELETE FROM Promotions; DELETE FROM Banners;

-- ── Categories ──
IF NOT EXISTS (SELECT 1 FROM Categories WHERE NameEn = 'Women')
BEGIN
    INSERT INTO Categories (NameAr, NameEn, IconClass, SortOrder, IsActive)
    VALUES
        (N'نسائي', 'Women', 'fas fa-female', 1, 1),
        (N'رجالي', 'Men', 'fas fa-male', 2, 1),
        (N'أطفال', 'Kids', 'fas fa-child', 3, 1),
        (N'مخمرية', 'Bakhoor', 'fas fa-fire', 4, 1),
        (N'معمول', 'Ma''amoul', 'fas fa-gem', 5, 1),
        (N'يونيسكس', 'Unisex', 'fas fa-infinity', 6, 1);
END
GO

-- ── Products ──
-- (Using EF Core seeder via DbSeeder.cs is preferred)
-- Below is manual SQL equivalent for reference

DECLARE @WomenId INT = (SELECT Id FROM Categories WHERE NameEn = 'Women');
DECLARE @MenId   INT = (SELECT Id FROM Categories WHERE NameEn = 'Men');
DECLARE @UnisexId INT = (SELECT Id FROM Categories WHERE NameEn = 'Unisex');
DECLARE @BakhoorId INT = (SELECT Id FROM Categories WHERE NameEn = 'Bakhoor');
DECLARE @MaamoulId INT = (SELECT Id FROM Categories WHERE NameEn = 'Ma''amoul');
DECLARE @KidsId INT = (SELECT Id FROM Categories WHERE NameEn = 'Kids');

IF NOT EXISTS (SELECT 1 FROM Products WHERE NameEn = 'Abdul Rahman')
BEGIN
    INSERT INTO Products
        (NameAr, NameEn, DescriptionAr, DescriptionEn, PriceKWD, OriginalPriceKWD,
         CategoryId, ImageUrl, Gender, Concentration, ScentNotesJson, SizesJson,
         IsFeatured, IsBestSeller, IsNewArrival, IsActive, StockQuantity, Rating, ReviewCount, CreatedAt, UpdatedAt)
    VALUES
    -- 1. عبدالرحمن
    (N'عبدالرحمن', 'Abdul Rahman',
     N'عطر ملكي استثنائي يحمل توقيع عبدالرحمن أبو الذهب الشخصي.',
     'A royal exceptional fragrance bearing Abdul Rahman Abu Al Dhahab''s personal signature.',
     9.000, 12.000, @UnisexId, '/images/products/abdulrahman.jpg', 'Unisex', 'EDP',
     '{"top":["فواكه طازجة","بهارات"],"middle":["ورد نادر","زعفران ذهبي"],"base":["مسك فاخر","عنبر"]}',
     '[{"size":"65ml","price":7.000},{"size":"75ml","price":9.000},{"size":"110ml","price":12.000}]',
     1, 1, 0, 1, 50, 5.0, 128, GETUTCDATE(), GETUTCDATE()),

    -- 2. بخور الذهب
    (N'بخور الذهب', 'Bakhoor Al Dhahab',
     N'بخور فاخر مستوحى من أعرق التقاليد الخليجية.',
     'Luxurious incense inspired by the finest Gulf traditions.',
     7.500, NULL, @BakhoorId, '/images/products/bakhoor-dhahab.jpg', 'Unisex', 'Bukhoor',
     '{"top":["دخان عود"],"middle":["صندل هندي","عود كمبودي"],"base":["مسك","عنبر أبيض"]}',
     '[{"size":"100g","price":5.500},{"size":"250g","price":7.500}]',
     1, 0, 0, 1, 75, 4.9, 89, GETUTCDATE(), GETUTCDATE()),

    -- 3. الماس
    (N'الماس', 'Al Mas',
     N'عطر نسائي ماسي يشع بفخامة لا تضاهى.',
     'A diamond-like women''s fragrance radiating unmatched luxury.',
     8.500, NULL, @WomenId, '/images/products/almas.jpg', 'Female', 'EDP',
     '{"top":["فراولة","كمثرى"],"middle":["ورد طائفي","فل هندي"],"base":["مسك أبيض","عود مضيء"]}',
     '[{"size":"65ml","price":6.500},{"size":"75ml","price":8.500},{"size":"110ml","price":11.000}]',
     1, 0, 1, 1, 40, 4.8, 67, GETUTCDATE(), GETUTCDATE()),

    -- 4. الفهد
    (N'الفهد', 'Al Fahad',
     N'عطر رجالي مفترس وجذاب يجسد قوة الفهد الأنيق.',
     'A predatory and captivating men''s fragrance.',
     8.000, NULL, @MenId, '/images/products/alfahad.jpg', 'Male', 'EDP',
     '{"top":["فلفل أسود","برغموت"],"middle":["خشب الأرز","جلد"],"base":["عود رجالي","مسك داكن"]}',
     '[{"size":"65ml","price":6.000},{"size":"75ml","price":8.000},{"size":"110ml","price":10.500}]',
     1, 1, 0, 1, 35, 4.9, 95, GETUTCDATE(), GETUTCDATE()),

    -- 5. فولكانو
    (N'فولكانو', 'Volcano',
     N'عطر استثنائي يحاكي قوة البركان.',
     'An exceptional fragrance mimicking the power of a volcano.',
     9.000, NULL, @UnisexId, '/images/products/volcano.jpg', 'Unisex', 'Parfum',
     '{"top":["قرفة","فلفل وردي"],"middle":["عنبر","زهرة اللوتس"],"base":["لبان","عود لاوسي"]}',
     '[{"size":"65ml","price":7.000},{"size":"75ml","price":9.000},{"size":"110ml","price":12.000}]',
     1, 0, 1, 1, 30, 4.7, 54, GETUTCDATE(), GETUTCDATE()),

    -- 6. نور الصباح
    (N'نور الصباح', 'Noor Al Sabah',
     N'عطر نسائي منعش وزهري يجسد نقاء الصباح الكويتي.',
     'A fresh floral women''s fragrance.',
     6.500, NULL, @WomenId, '/images/products/noor-sabah.jpg', 'Female', 'EDT',
     '{"top":["تفاح أخضر","ليمون"],"middle":["ياسمين عربي","ورد بلغاري"],"base":["مسك أبيض","خشب سيدر"]}',
     '[{"size":"65ml","price":5.000},{"size":"75ml","price":6.500}]',
     0, 0, 1, 1, 60, 4.6, 42, GETUTCDATE(), GETUTCDATE()),

    -- 7. الليل العربي
    (N'الليل العربي', 'Arabian Night',
     N'رحلة عطرية عبر ليالي الجزيرة العربية.',
     'A fragrance journey through Arabian nights.',
     8.500, NULL, @MenId, '/images/products/arabian-night.jpg', 'Male', 'EDP',
     '{"top":["زعفران أحمر","هيل"],"middle":["ورد دمشقي","عود هندي"],"base":["عنبر رمادي","مسك أسود"]}',
     '[{"size":"65ml","price":6.500},{"size":"75ml","price":8.500},{"size":"110ml","price":11.500}]',
     0, 1, 0, 1, 25, 5.0, 110, GETUTCDATE(), GETUTCDATE()),

    -- 8. ملكة الزهور
    (N'ملكة الزهور', 'Queen of Flowers',
     N'عطر ملكي لامرأة تستحق التاج.',
     'A royal fragrance for a woman who deserves the crown.',
     9.000, NULL, @WomenId, '/images/products/queen-flowers.jpg', 'Female', 'Parfum',
     '{"top":["إيلانغ","نارنج"],"middle":["ورد جوري","ياسمين سامباك"],"base":["فانيليا","مسك ناعم"]}',
     '[{"size":"65ml","price":7.000},{"size":"75ml","price":9.000},{"size":"110ml","price":12.500}]',
     1, 0, 0, 1, 20, 4.8, 76, GETUTCDATE(), GETUTCDATE()),

    -- 9. صحراء الذهب
    (N'صحراء الذهب', 'Golden Desert',
     N'عطر يأخذك في رحلة عبر الصحراء العربية الذهبية.',
     'A fragrance taking you on a journey through the golden Arabian desert.',
     7.000, NULL, @UnisexId, '/images/products/golden-desert.jpg', 'Unisex', 'EDP',
     '{"top":["رمل ذهبي","أوريس"],"middle":["عنبر صحراوي","عود"],"base":["مسك صحراوي","لبان"]}',
     '[{"size":"65ml","price":5.500},{"size":"75ml","price":7.000},{"size":"110ml","price":9.500}]',
     0, 0, 0, 1, 45, 4.7, 58, GETUTCDATE(), GETUTCDATE()),

    -- 10. أمير الشرق
    (N'أمير الشرق', 'Prince of the East',
     N'عطر رجالي راقٍ يليق بأمراء الشرق.',
     'A refined men''s fragrance befitting Eastern princes.',
     8.000, NULL, @MenId, '/images/products/amir-sharq.jpg', 'Male', 'EDP',
     '{"top":["زعفران مراكشي","هيل خليجي"],"middle":["عود بروني","جلد نادر"],"base":["مسك ملكي","كهرمان"]}',
     '[{"size":"65ml","price":6.000},{"size":"75ml","price":8.000},{"size":"110ml","price":11.000}]',
     0, 1, 1, 1, 30, 4.9, 83, GETUTCDATE(), GETUTCDATE()),

    -- 11. طفل الملوك
    (N'طفل الملوك', 'Little Royals',
     N'عطر أطفال آمن وناعم بعطر الحلوى والفواكه.',
     'A safe and gentle children''s fragrance.',
     5.000, NULL, @KidsId, '/images/products/kids-royal.jpg', 'Kids', 'EDT',
     '{"top":["تفاح حلو","فراولة"],"middle":["حلوى قطن"],"base":["مسك أطفال","فانيليا"]}',
     '[{"size":"50ml","price":5.000}]',
     0, 0, 0, 1, 80, 4.8, 37, GETUTCDATE(), GETUTCDATE()),

    -- 12. معمول الذهب
    (N'معمول الذهب', 'Ma''amoul Al Dhahab',
     N'معمول عطري فاخر معجون بالعود الكمبودي والورد الطائفي.',
     'Luxury perfume paste crafted with Cambodian oud and Taif rose.',
     9.000, NULL, @MaamoulId, '/images/products/maamoul-dhahab.jpg', 'Unisex', 'Parfum',
     '{"top":["عود كمبودي"],"middle":["ورد طائفي","زعفران"],"base":["مسك حيواني","عنبر"]}',
     '[{"size":"25g","price":6.000},{"size":"50g","price":9.000}]',
     1, 1, 0, 1, 15, 5.0, 145, GETUTCDATE(), GETUTCDATE());
END
GO

-- ── Promotions ──
IF NOT EXISTS (SELECT 1 FROM Promotions WHERE Code = 'WELCOME20')
BEGIN
    INSERT INTO Promotions (Code, DescriptionAr, DescriptionEn, DiscountType, DiscountValue, MinOrderAmount, IsActive, CreatedAt)
    VALUES
        ('WELCOME20', N'خصم 20% للعملاء الجدد', '20% off for new customers', 'Percentage', 20, 10.000, 1, GETUTCDATE()),
        ('GOLD2025', N'خصم 1.500 د.ك على الطلبات فوق 20 د.ك', '1.5 KD off orders over 20 KD', 'Fixed', 1.500, 20.000, 1, GETUTCDATE()),
        ('SUMMER30', N'خصم صيف 30%', '30% Summer Discount', 'Percentage', 30, 15.000, 1, GETUTCDATE());
END
GO

-- ── Banners ──
IF NOT EXISTS (SELECT 1 FROM Banners WHERE Position = 'Hero')
BEGIN
    INSERT INTO Banners (TitleAr, TitleEn, SubtitleAr, SubtitleEn, ButtonTextAr, ButtonTextEn, LinkUrl, Position, SortOrder, IsActive)
    VALUES
        (N'عطرك يعبر عن شخصيتك', 'Your Scent Defines You',
         N'اكتشف مجموعة ماء الذهب الكويتية الفاخرة', 'Discover Maa Al Dhahab Luxury Collection',
         N'تسوق الآن', 'Shop Now', '/Shop', 'Hero', 1, 1),
        (N'خصم 20% لفترة محدودة', '20% Off Limited Time',
         N'على جميع عطور المجموعة الجديدة', 'On all new collection fragrances',
         N'احصل على الخصم', 'Get Discount', '/Shop', 'Promo', 1, 1);
END
GO

PRINT 'Seed data inserted successfully for Maa Al Dhahab! ✓';
GO
