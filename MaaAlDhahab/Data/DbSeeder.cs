using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MaaAlDhahab.Models;

namespace MaaAlDhahab.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Seed Roles
            string[] roles = { "Admin", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed Admin User
            if (await userManager.FindByEmailAsync("admin@maaAldhahab.com") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@maaAldhahab.com",
                    Email = "admin@maaAldhahab.com",
                    FullNameAr = "مدير النظام",
                    FullNameEn = "System Admin",
                    EmailConfirmed = true,
                    PhoneNumber = "+96512345678"
                };
                var result = await userManager.CreateAsync(admin, "Admin@123456");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Seed Categories
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new() { NameAr = "نسائي", NameEn = "Women", IconClass = "fas fa-female", SortOrder = 1 },
                    new() { NameAr = "رجالي", NameEn = "Men", IconClass = "fas fa-male", SortOrder = 2 },
                    new() { NameAr = "أطفال", NameEn = "Kids", IconClass = "fas fa-child", SortOrder = 3 },
                    new() { NameAr = "مخمرية", NameEn = "Bakhoor", IconClass = "fas fa-fire", SortOrder = 4 },
                    new() { NameAr = "معمول", NameEn = "Ma'amoul", IconClass = "fas fa-gem", SortOrder = 5 },
                    new() { NameAr = "يونيسكس", NameEn = "Unisex", IconClass = "fas fa-infinity", SortOrder = 6 }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Seed Products
            if (!context.Products.Any())
            {
                var cats = await context.Categories.ToListAsync();
                int womenId = cats.First(c => c.NameEn == "Women").Id;
                int menId = cats.First(c => c.NameEn == "Men").Id;
                int unisexId = cats.First(c => c.NameEn == "Unisex").Id;
                int bakhoorId = cats.First(c => c.NameEn == "Bakhoor").Id;
                int maAmoulId = cats.First(c => c.NameEn == "Ma'amoul").Id;
                int kidsId = cats.First(c => c.NameEn == "Kids").Id;

                var products = new List<Product>
                {
                    new()
                    {
                        NameAr = "عبدالرحمن",
                        NameEn = "Abdul Rahman",
                        DescriptionAr = "عطر ملكي استثنائي يحمل توقيع عبدالرحمن أبو الذهب الشخصي. مزيج من أرقى المكونات الشرقية والغربية، يبدأ بانفجار من الفواكه الطازجة والبهارات، ليتطور إلى قلب من الورود النادرة والزعفران الذهبي، وينتهي بقاعدة عميقة من المسك الفاخر والعنبر.",
                        DescriptionEn = "A royal exceptional fragrance bearing Abdul Rahman Abu Al Dhahab's personal signature. A blend of the finest Eastern and Western ingredients.",
                        PriceKWD = 9.000m,
                        OriginalPriceKWD = 12.000m,
                        CategoryId = unisexId,
                        ImageUrl = "/images/products/abdulrahman.jpg",
                        ScentNotesJson = """{"top": ["فواكه طازجة", "بهارات", "برغموت"], "middle": ["ورد نادر", "زعفران ذهبي", "ياسمين"], "base": ["مسك فاخر", "عنبر", "عود كمبودي"]}""",
                        SizesJson = """[{"size": "65ml", "price": 7.000}, {"size": "75ml", "price": 9.000}, {"size": "110ml", "price": 12.000}]""",
                        Gender = "Unisex",
                        Concentration = "EDP",
                        IsFeatured = true,
                        IsBestSeller = true,
                        Rating = 5.0,
                        ReviewCount = 128,
                        StockQuantity = 50
                    },
                    new()
                    {
                        NameAr = "بخور الذهب",
                        NameEn = "Bakhoor Al Dhahab",
                        DescriptionAr = "بخور فاخر مستوحى من أعرق التقاليد الخليجية. مزيج ساحر من العود الأصيل، الصندل، والمسك الثمين يملأ المكان بأجواء ملكية.",
                        DescriptionEn = "Luxurious incense inspired by the finest Gulf traditions. A magical blend of authentic oud, sandalwood, and precious musk.",
                        PriceKWD = 7.500m,
                        CategoryId = bakhoorId,
                        ImageUrl = "/images/products/bakhoor-dhahab.jpg",
                        ScentNotesJson = """{"top": ["دخان عود", "كافور"], "middle": ["صندل هندي", "عود كمبودي"], "base": ["مسك", "عنبر أبيض"]}""",
                        SizesJson = """[{"size": "100g", "price": 5.500}, {"size": "250g", "price": 7.500}]""",
                        Gender = "Unisex",
                        Concentration = "Bukhoor",
                        IsFeatured = true,
                        Rating = 4.9,
                        ReviewCount = 89,
                        StockQuantity = 75
                    },
                    new()
                    {
                        NameAr = "الماس",
                        NameEn = "Al Mas (Diamond)",
                        DescriptionAr = "عطر نسائي ماسي يشع بفخامة لا تضاهى. يفتتح بزهور الربيع الناضرة ليكشف عن قلب من الورد الطائفي والفل الهندي، وينتهي بقاعدة ملكية من المسك الأبيض والعود المضيء.",
                        DescriptionEn = "A diamond-like women's fragrance radiating unmatched luxury. Opens with blooming spring flowers revealing a heart of Taif rose.",
                        PriceKWD = 8.500m,
                        CategoryId = womenId,
                        ImageUrl = "/images/products/almas.jpg",
                        ScentNotesJson = """{"top": ["فراولة", "كمثرى", "تفاح"], "middle": ["ورد طائفي", "فل هندي", "زنبق"], "base": ["مسك أبيض", "عود مضيء", "خشب الصندل"]}""",
                        SizesJson = """[{"size": "65ml", "price": 6.500}, {"size": "75ml", "price": 8.500}, {"size": "110ml", "price": 11.000}]""",
                        Gender = "Female",
                        Concentration = "EDP",
                        IsFeatured = true,
                        IsNewArrival = true,
                        Rating = 4.8,
                        ReviewCount = 67,
                        StockQuantity = 40
                    },
                    new()
                    {
                        NameAr = "الفهد",
                        NameEn = "Al Fahad (The Leopard)",
                        DescriptionAr = "عطر رجالي مفترس وجذاب يجسد قوة الفهد الأنيق. يبدأ بتوابل حارة وحمضيات نارية، يكشف عن قلب خشبي عميق، ويختم بقاعدة من العود الرجالي والمسك الداكن.",
                        DescriptionEn = "A predatory and captivating men's fragrance embodying the elegant leopard's power.",
                        PriceKWD = 8.000m,
                        CategoryId = menId,
                        ImageUrl = "/images/products/alfahad.jpg",
                        ScentNotesJson = """{"top": ["فلفل أسود", "برغموت", "ليمون"], "middle": ["خشب الأرز", "صندل أسترالي", "جلد"], "base": ["عود رجالي", "مسك داكن", "لبان"]}""",
                        SizesJson = """[{"size": "65ml", "price": 6.000}, {"size": "75ml", "price": 8.000}, {"size": "110ml", "price": 10.500}]""",
                        Gender = "Male",
                        Concentration = "EDP",
                        IsFeatured = true,
                        IsBestSeller = true,
                        Rating = 4.9,
                        ReviewCount = 95,
                        StockQuantity = 35
                    },
                    new()
                    {
                        NameAr = "فولكانو",
                        NameEn = "Volcano",
                        DescriptionAr = "عطر استثنائي يحاكي قوة البركان. انفجار من البهارات الحارة والتوابل النادرة يتطور إلى قلب من الزهور الإكزوتيكية، ليهدأ على قاعدة بركانية من اللبان والعنبر الداكن.",
                        DescriptionEn = "An exceptional fragrance mimicking the power of a volcano. An explosion of hot spices and rare ingredients.",
                        PriceKWD = 9.000m,
                        CategoryId = unisexId,
                        ImageUrl = "/images/products/volcano.jpg",
                        ScentNotesJson = """{"top": ["قرفة سيلانية", "فلفل وردي", "زنجبيل"], "middle": ["عنبر", "زهرة اللوتس", "ياسمين حجازي"], "base": ["لبان", "باتشولي", "عود لاوسي"]}""",
                        SizesJson = """[{"size": "65ml", "price": 7.000}, {"size": "75ml", "price": 9.000}, {"size": "110ml", "price": 12.000}]""",
                        Gender = "Unisex",
                        Concentration = "Parfum",
                        IsFeatured = true,
                        IsNewArrival = true,
                        Rating = 4.7,
                        ReviewCount = 54,
                        StockQuantity = 30
                    },
                    new()
                    {
                        NameAr = "نور الصباح",
                        NameEn = "Noor Al Sabah",
                        DescriptionAr = "عطر نسائي منعش وزهري يجسد نقاء الصباح الكويتي. ترتاح فيه الروح وتتجدد بنسمات الياسمين والورد البلغاري النقي.",
                        DescriptionEn = "A fresh floral women's fragrance embodying the purity of Kuwaiti morning.",
                        PriceKWD = 6.500m,
                        CategoryId = womenId,
                        ImageUrl = "/images/products/noor-sabah.jpg",
                        ScentNotesJson = """{"top": ["تفاح أخضر", "ليمون أيوني", "رياح"], "middle": ["ياسمين عربي", "ورد بلغاري", "فاوانيا"], "base": ["مسك أبيض", "خشب سيدر", "أمبروكسان"]}""",
                        SizesJson = """[{"size": "65ml", "price": 5.000}, {"size": "75ml", "price": 6.500}]""",
                        Gender = "Female",
                        Concentration = "EDT",
                        IsNewArrival = true,
                        Rating = 4.6,
                        ReviewCount = 42,
                        StockQuantity = 60
                    },
                    new()
                    {
                        NameAr = "الليل العربي",
                        NameEn = "Arabian Night",
                        DescriptionAr = "رحلة عطرية عبر ليالي الجزيرة العربية. عود فاخر مع التوابل الشرقية وقلب من الورود الدمشقية وقاعدة من العنبر والمسك الأسود.",
                        DescriptionEn = "A fragrance journey through Arabian nights. Luxury oud with Eastern spices and Damascus roses.",
                        PriceKWD = 8.500m,
                        CategoryId = menId,
                        ImageUrl = "/images/products/arabian-night.jpg",
                        ScentNotesJson = """{"top": ["زعفران أحمر", "هيل", "توابل"], "middle": ["ورد دمشقي", "عود هندي", "بخور"], "base": ["عنبر رمادي", "مسك أسود", "لبان ذكر"]}""",
                        SizesJson = """[{"size": "65ml", "price": 6.500}, {"size": "75ml", "price": 8.500}, {"size": "110ml", "price": 11.500}]""",
                        Gender = "Male",
                        Concentration = "EDP",
                        IsBestSeller = true,
                        Rating = 5.0,
                        ReviewCount = 110,
                        StockQuantity = 25
                    },
                    new()
                    {
                        NameAr = "ملكة الزهور",
                        NameEn = "Queen of Flowers",
                        DescriptionAr = "عطر ملكي لامرأة تستحق التاج. مزيج أسطوري من أندر أزهار العالم في تناغم مثالي مع الإيلانغ والفانيليا الطازجة.",
                        DescriptionEn = "A royal fragrance for a woman who deserves the crown. A legendary blend of the world's rarest flowers.",
                        PriceKWD = 9.000m,
                        CategoryId = womenId,
                        ImageUrl = "/images/products/queen-flowers.jpg",
                        ScentNotesJson = """{"top": ["إيلانغ إيلانغ", "نارنج", "برغموت لذيذ"], "middle": ["ورد جوري", "ياسمين سامباك", "ايريس"], "base": ["فانيليا طازجة", "مسك ناعم", "خشب الأرز"]}""",
                        SizesJson = """[{"size": "65ml", "price": 7.000}, {"size": "75ml", "price": 9.000}, {"size": "110ml", "price": 12.500}]""",
                        Gender = "Female",
                        Concentration = "Parfum",
                        IsFeatured = true,
                        Rating = 4.8,
                        ReviewCount = 76,
                        StockQuantity = 20
                    },
                    new()
                    {
                        NameAr = "صحراء الذهب",
                        NameEn = "Golden Desert",
                        DescriptionAr = "عطر يأخذك في رحلة عبر الصحراء العربية الذهبية. دفء الرمال وبرودة الليل في زجاجة واحدة فخمة.",
                        DescriptionEn = "A fragrance taking you on a journey through the golden Arabian desert.",
                        PriceKWD = 7.000m,
                        CategoryId = unisexId,
                        ImageUrl = "/images/products/golden-desert.jpg",
                        ScentNotesJson = """{"top": ["رمل ذهبي", "أوريس", "صندل"], "middle": ["عنبر صحراوي", "توابل", "عود"], "base": ["مسك صحراوي", "خشب جوز الهند", "لبان"]}""",
                        SizesJson = """[{"size": "65ml", "price": 5.500}, {"size": "75ml", "price": 7.000}, {"size": "110ml", "price": 9.500}]""",
                        Gender = "Unisex",
                        Concentration = "EDP",
                        Rating = 4.7,
                        ReviewCount = 58,
                        StockQuantity = 45
                    },
                    new()
                    {
                        NameAr = "أمير الشرق",
                        NameEn = "Prince of the East",
                        DescriptionAr = "عطر رجالي راقٍ يليق بأمراء الشرق. مزيج شرقي فاخر من التوابل الذهبية والعود الملكي والمسك النفيس.",
                        DescriptionEn = "A refined men's fragrance befitting Eastern princes. A luxurious Eastern blend of golden spices.",
                        PriceKWD = 8.000m,
                        CategoryId = menId,
                        ImageUrl = "/images/products/amir-sharq.jpg",
                        ScentNotesJson = """{"top": ["زعفران مراكشي", "فلفل أبيض", "هيل خليجي"], "middle": ["عود بروني", "جلد نادر", "دخان تبغ"], "base": ["مسك ملكي", "كهرمان", "أمبرغريس"]}""",
                        SizesJson = """[{"size": "65ml", "price": 6.000}, {"size": "75ml", "price": 8.000}, {"size": "110ml", "price": 11.000}]""",
                        Gender = "Male",
                        Concentration = "EDP",
                        IsNewArrival = true,
                        IsBestSeller = true,
                        Rating = 4.9,
                        ReviewCount = 83,
                        StockQuantity = 30
                    },
                    new()
                    {
                        NameAr = "طفل الملوك",
                        NameEn = "Little Royals",
                        DescriptionAr = "عطر أطفال آمن وناعم بعطر الحلوى والفواكه المحببة للأطفال. مصنوع بأعلى معايير السلامة.",
                        DescriptionEn = "A safe and gentle children's fragrance with candy and fruity scents kids love.",
                        PriceKWD = 5.000m,
                        CategoryId = kidsId,
                        ImageUrl = "/images/products/kids-royal.jpg",
                        ScentNotesJson = """{"top": ["تفاح حلو", "فراولة"], "middle": ["حلوى قطن", "شوكولاتة بيضاء"], "base": ["مسك أطفال", "فانيليا"]}""",
                        SizesJson = """[{"size": "50ml", "price": 5.000}]""",
                        Gender = "Kids",
                        Concentration = "EDT",
                        Rating = 4.8,
                        ReviewCount = 37,
                        StockQuantity = 80
                    },
                    new()
                    {
                        NameAr = "معمول الذهب",
                        NameEn = "Ma'amoul Al Dhahab",
                        DescriptionAr = "معمول عطري فاخر معجون بالعود الكمبودي والورد الطائفي، يتشكّل على يد أستاذ العطور عبدالرحمن أبو الذهب بأسرار تتوارث جيلاً بعد جيل.",
                        DescriptionEn = "Luxury perfume paste crafted with Cambodian oud and Taif rose by Master Perfumer Abdul Rahman Abu Al Dhahab.",
                        PriceKWD = 9.000m,
                        CategoryId = maAmoulId,
                        ImageUrl = "/images/products/maamoul-dhahab.jpg",
                        ScentNotesJson = """{"top": ["عود كمبودي"], "middle": ["ورد طائفي", "زعفران"], "base": ["مسك حيواني", "عنبر"]}""",
                        SizesJson = """[{"size": "25g", "price": 6.000}, {"size": "50g", "price": 9.000}]""",
                        Gender = "Unisex",
                        Concentration = "Parfum",
                        IsFeatured = true,
                        IsBestSeller = true,
                        Rating = 5.0,
                        ReviewCount = 145,
                        StockQuantity = 15
                    }
                };
                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }

            // Seed Banners
            if (!context.Banners.Any())
            {
                context.Banners.AddRange(
                    new Banner
                    {
                        TitleAr = "عطرك يعبر عن شخصيتك",
                        TitleEn = "Your Scent Defines You",
                        SubtitleAr = "اكتشف مجموعة ماء الذهب - فخامة لا مثيل لها من الكويت",
                        SubtitleEn = "Discover Maa Al Dhahab Collection - Unparalleled Luxury from Kuwait",
                        ButtonTextAr = "تسوق الآن",
                        ButtonTextEn = "Shop Now",
                        LinkUrl = "/Shop",
                        Position = "Hero",
                        SortOrder = 1,
                        IsActive = true
                    },
                    new Banner
                    {
                        TitleAr = "عرض خاص - خصم 20%",
                        TitleEn = "Special Offer - 20% Off",
                        SubtitleAr = "على جميع العطور النسائية لفترة محدودة",
                        SubtitleEn = "On all women's fragrances for a limited time",
                        ButtonTextAr = "احصل على الخصم",
                        ButtonTextEn = "Get Discount",
                        LinkUrl = "/Shop?gender=Female",
                        Position = "Promo",
                        SortOrder = 1,
                        IsActive = true
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed Promotions
            if (!context.Promotions.Any())
            {
                context.Promotions.AddRange(
                    new Promotion
                    {
                        Code = "WELCOME20",
                        DescriptionAr = "خصم 20% للعملاء الجدد",
                        DescriptionEn = "20% discount for new customers",
                        DiscountType = "Percentage",
                        DiscountValue = 20,
                        MinOrderAmount = 10.000m,
                        UsageLimit = 1000,
                        IsActive = true
                    },
                    new Promotion
                    {
                        Code = "GOLD2025",
                        DescriptionAr = "خصم 1.500 د.ك على الطلبات فوق 20 د.ك",
                        DescriptionEn = "1.5 KD off orders over 20 KD",
                        DiscountType = "Fixed",
                        DiscountValue = 1.500m,
                        MinOrderAmount = 20.000m,
                        IsActive = true
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed Reviews/Testimonials
            if (!context.Reviews.Any())
            {
                var product = await context.Products.FirstOrDefaultAsync();
                if (product != null)
                {
                    context.Reviews.AddRange(
                        new Review { ProductId = product.Id, ReviewerName = "أم سعد الكويتية", Comment = "والله عطر يجنن، كل من اشتممته سألني عن اسمه. مستحيل أغير عطري بعد اليوم 🌹", Rating = 5, IsApproved = true },
                        new Review { ProductId = product.Id, ReviewerName = "Mohammed Al-Rashidi", Comment = "Absolutely stunning fragrance! The quality is on par with any international luxury brand. Worth every fils!", Rating = 5, IsApproved = true },
                        new Review { ProductId = product.Id, ReviewerName = "فهد العتيبي", Comment = "ثبات العطر ممتاز جداً، من الصبح للليل. الرائحة فخمة ومميزة", Rating = 5, IsApproved = true }
                    );
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
