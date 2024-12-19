using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GamesDetailsPageController : ControllerBase
{
    private readonly DataContext dataContext;

    public GamesDetailsPageController(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    [HttpGet("pageSize={pageSize}&page={page}")]
    public async Task<ActionResult<IEnumerable<Game>>> GetAllGamesAsync(int pageSize, int page)
    {
        if (dataContext.GamesLocalizations.Count() == 0)
        {
            dataContext.AddRange(new GameLocalization { Title = "Полностью на русском языке", Url = "language/fully-localized-to-russian" },
                new GameLocalization { Title = "Субтитры на русском языке", Url = "language/russian-subtitles" },
                new GameLocalization { Title = "Нет русского языка", Url = "language/without-russian-language" }
                );
            await dataContext.SaveChangesAsync();
        }
        if (this.dataContext.GamesDevelopers.Count() == 0)
        {
            dataContext.AddRange(
       [
                new Domain.GameDeveloper
            {
                Name = "Naughty Dog",
                Url = "games-developers/naughty-dog"
            },
                new Domain.GameDeveloper
            {
                Name = "CD Projekt RED",
                Url = "games-developers/cd-project-red"
            },
            new Domain.GameDeveloper
            {
                Name = "Santa Monica Studio",
                Url = "games-developers/santa-monica-studio"
            },
            new Domain.GameDeveloper
            {
                Name = "Larian Studios",
                Url = "games-developers/larian-studios"
            },
            new Domain.GameDeveloper
            {
                Name = "Rockstar Games",
                Url = "games-developers/rockstar-games"
            },
            new Domain.GameDeveloper
            {
                Name = "Quantic Dream",
                Url = "games-developers/quantic-dream"
            },
            new Domain.GameDeveloper
            {
                Name = "Supermassive Games",
                Url = "games-developers/supermassive-games"
            },
            new Domain.GameDeveloper
            {
                Name = "Guerilla Games",
                Url = "games-developers/guerilla-games"
            },
            new Domain.GameDeveloper
            {
                Name = "Insomniac Games",
                Url = "games-developers/insomniac-games"
            },
                new Domain.GameDeveloper
            {
                Name = "Nintendo",
                Url = "games-developers/nintendo"
            },
            new Domain.GameDeveloper
            {
                Name = "Capcom",
                Url = "games-developers/capcom"
            },
            new Domain.GameDeveloper
            {
                Name = "Nintendo EAD",
                Url = "games-developers/nintendo-ead"
            },
            new Domain.GameDeveloper
            {
                Name = "FromSoftware",
                Url = "games-developers/fromsoftware"
            },
            new Domain.GameDeveloper
            {
                Name = "Valve",
                Url = "games-developers/valve"
            },
                new Domain.GameDeveloper
            {
                Name = "Bluepoint Games",
                Url = "games-developers/bluepoint-games"
            },
            new Domain.GameDeveloper
            {
                Name = "Epic Games",
                Url = "games-developers/epic-games"
            },
            new Domain.GameDeveloper
            {
                Name = "ZAUM",
                Url = "games-developers/zaum"
            },
            new Domain.GameDeveloper
            {
                Name = "4A Games",
                Url = "games-developers/4a-games"
            },
            new Domain.GameDeveloper
            {
                Name = "Moon Studios",
                Url = "games-developers/moon-studios"
            },
            new Domain.GameDeveloper
            {
                Name = "Monster Games",
                Url = "games-developers/monster-games"
            },
            new Domain.GameDeveloper
            {
                Name = "Playground Games",
                Url = "games-developers/playground-games"
            },
            new Domain.GameDeveloper
            {
                Name = "Irrational Games",
                Url = "games-developers/irrational-games"
            },
            new Domain.GameDeveloper
            {
                Name = "Atlus Co.",
                Url = "games-developers/atlus-co"
            },
            new GameDeveloper
            {
                Name = "Crystal Dynamics",
                Url = "games-developers/crystal-dynamics"
            },
            new GameDeveloper
            {
                Name = "Sony Santa Monica",
                Url = "games-developers/sony-santa-monica"
            },
            new GameDeveloper
            {
                Name = "Kojima Productions",
                Url = "games-developers/kojima-productions"
            },
            new GameDeveloper
            {
                Name = "Team Asobi",
                Url = "games-developers/team-asobi"
            },
            new GameDeveloper
            {
                Name = "Ubisoft Montpellier",
                Url = "games-developers/ubisoft-montpellier"
            },
                new GameDeveloper
            {
                Name = "Rocksteady Studios",
                Url = "games-developers/rocksteady-studios"
            },
                new GameDeveloper
            {
                Name = "Media Molecule",
                Url = "games-developers/media-molecule"
            },
                new GameDeveloper
            {
                Name = "IO Interactive",
                Url = "games-developers/io-interactive"
            },
       ]);
            await dataContext.SaveChangesAsync();
        }
        if (this.dataContext.Critics.Count() == 0)
        {
            this.dataContext.AddRange(
            [
                new Critic { Name = "Igromania", Url = "critics/igromania" },
                new Critic { Name = "Stopgame", Url = "critics/stopgame"},
                new Critic { Name = "iXBT", Url = "critics/ixbt"},
                new Critic { Name = "Навигатор игрового мира", Url = "critics/games-world-navigator" },
                new Critic { Name = "ТАК ОСТРО", Url = "critics/so-spicy" },
                new Critic { Name = "Антон Логвинов", Url = "critics/anton-logvinov" },
                new Critic { Name = "Алексей Макаренков", Url="critics/aleksey-makarenkov" }
            ]);
            await dataContext.SaveChangesAsync();
        }
        if (dataContext.GamesPublishers.Count() == 0)
        {
            dataContext.AddRange([
                new GamePublisher { Name= "Sony Computer Entertainment", Url="publishers/sony-computer-entertainment" },
                new GamePublisher{ Name="Namco Bandai Games", Url="publishers/namco-bandai-games"},
                new GamePublisher{Name="Rockstar Games", Url = "publishers/rockstar-games"},
                new GamePublisher{Name="Larian Studios", Url="publishers/larian-studios"},
                new GamePublisher{Name="Capcom", Url="publishers/capcom"},
                new GamePublisher{Name="CD Projekt RED", Url="publishers/cd-project-red"},
                new GamePublisher{Name="Nintendo", Url="publishers/nintendo"},
                new GamePublisher{Name="Bandai Namco Entertainment", Url="publishers/bandai-namco-entertainment"},
                new GamePublisher{Name="Valve", Url="publishers/valve"},
                new GamePublisher{Name="Sony Enteractive Entertainment", Url="publishers/sony-interactive-entertainment"},
                new GamePublisher{Name="Microsoft Game Studios", Url="publishers/microsoft-game-studios"},
                new GamePublisher{Name="ZAUM", Url="publishers/zaum"},
                new GamePublisher{Name="Deep Silver", Url="publishers/deep-silver"},
                new GamePublisher{Name="Microsoft Studios", Url="publishers/microsoft-studios"},
                new GamePublisher{Name="2K Games", Url="publishers/2K-Games"},
                new GamePublisher{Name="BANDAI NAMCO", Url="publishers/BANDAI-NAMCO"},
                new GamePublisher{Name="Atlus", Url="publishers/atlus"},
                new GamePublisher{Name="Square Enix", Url="publishers/square-enix"},
                new GamePublisher{Name="Konami", Url="publishers/konami"},
                new GamePublisher{Name="Ubisoft", Url="publishers/ubisoft"},
                new GamePublisher{Name="Warner Bros. Interactive Entertainment", Url="publishers/Warner-Bros-Interactive-Entertainment"},
                new GamePublisher{Name="PlayStation PC LLC", Url="publishers/PlayStation-PC-LLC"},
            ]);
            await dataContext.SaveChangesAsync();
        }

        if (dataContext.GamesPlatforms.Count() == 0)
        {
            dataContext.AddRange([
                new GamePlatform { Name= "PC", Url="platforms/PC" },
                new GamePlatform { Name="PlayStation 5", Url="platforms/PlayStation-5" },
                new GamePlatform { Name="Xbox Series X", Url = "platforms/Xbox-Series-X" },
                new GamePlatform { Name="PlayStation 4", Url="platforms/PlayStation-4" },
                new GamePlatform { Name="Xbox One", Url="platforms/Xbox-One" },
                new GamePlatform { Name="Nintendo Switch", Url="platforms/Nintendo-Switch" },
                new GamePlatform { Name="PlayStation 3", Url="platforms/PlayStation-3" },
                new GamePlatform { Name="Xbox 360", Url="platforms/Xbox-360" },
                new GamePlatform { Name="PS Vita", Url="platforms/PS-Vita" },
                new GamePlatform { Name="Wii U", Url="platforms/Wii-U" },
                new GamePlatform { Name="3DS", Url="platforms/3DS" }
            ]);
            await dataContext.SaveChangesAsync();
        }

        if (dataContext.GamesGenres.Count() == 0)
        {
            dataContext.AddRange([
                new GameGenre { Name= "Все", Url="genres/all" },
                new GameGenre { Name= "Экшены", Url="genres/action" },
                new GameGenre { Name="Шутеры", Url="genres/shooter" },
                new GameGenre { Name="Хорроры", Url = "genres/horrors" },
                new GameGenre { Name="РПГ", Url="genres/rpg" },
                new GameGenre { Name="Приключения", Url="genres/adventure" },
                new GameGenre { Name="Гонки", Url="genres/racings" },
                new GameGenre { Name="Стратегии", Url="genres/strategies" },
                new GameGenre { Name="Аркады", Url="genres/arcades" },
                new GameGenre { Name="MMO", Url="genres/MMOs" },
                new GameGenre { Name="Спорт", Url="genres/sport" },
                new GameGenre { Name="Файтинги", Url="genres/fightings" }
            ]);
            await dataContext.SaveChangesAsync();
        }

        if (dataContext.GamesLocalizations.Count() == 0)
        {
            dataContext.AddRange([
                new GameLocalization { Title= "Все", Url="localization/all" },
                new GameLocalization { Title= "Без русского языка", Url="localization/without-russian-language" },
                new GameLocalization { Title= "Субтитры на русском языке", Url="localization/russian-subtitles" },
                new GameLocalization { Title= "Полностью на русском языке", Url="localization/fully-localized-to-russian" },
            ]);
            await dataContext.SaveChangesAsync();
        }

        if (dataContext.Games.Count() == 0)
        {
            dataContext.AddRange([
                new Game
                {
                    Name="The Last of Us: Remastered",
                    ImageSource="https://metarankings.ru/images/uploads/2014/07/the-last-of-us-remastered-boxart-cover.jpg",
                    LocalizationId=1,
                    Score=9.4f,
                    Description="Это переиздание лучшей игры 2013 для PlayStation 3 — The Last of Us, которое будет иметь более большее разрешение 1080p, модели более высокого разрешения, новые тени и освещение, обновленные текстуры и множество других улучшений.\r\n\r\nСюжет игры отправит вас в пост-апокалиптический зараженный мир, где люди превращаются в зомби-мутантов, но не они ваш главный враг, а выжившие люди.\r\n\r\nВ обновленную версии для PS4 также войдут внутриигровое видео с комментариями от Нила Дракманна (сценарист, арт-директор игры) и тонна дополнительного контента из оригинала.",
                    ReleaseDate=DateTime.Parse("30 июля 2014")
                },
                new Game
                {
                    Name="The Last of Us (Одни из нас)",
                    ImageSource="https://metarankings.ru/images/uploads/2013/06/the-last-of-us-boxart-cover.jpg",
                    LocalizationId=1,
                    Score=9.0f,
                    Description="The Last of Us — приключенческая экшн игра от создателей серии Uncharted, студии Naughty Dog.\r\n\r\nСобытия The Last of Us развиваются спустя 20 лет после начала страшной эпидемии, которая превращает всех людей в зомби-мутантов. Джоел вместе с Элли отправляется в сложное путешествие через страну, бывшую некогда великой, но ставшей лишь заросшими зеленью руинами с жалкими остатками выживающих. Вам не раз предстоит с ними столкнуться и посмотреть на их жизнь в условиях страшного апокалипсиса.\r\n\r\nДевочка Элли будет всячески помогать вам на протяжении всего прохождения . Придется создавать предметы и использовать аптечки. Это эмоциональное произведение, где основным противником выступают сами люди, их нравы и совесть, а не зараженные.",
                    ReleaseDate=DateTime.Parse("14 июня 2013")
                },
                new Game
                {
                    Name="Uncharted 4: A Thief’s End\r\n",
                    ImageSource="https://metarankings.ru/images/uploads/2016/05/uncharted-4-a-thiefs-end-boxart-cover.jpg",
                    LocalizationId=1,
                    Score=8.7f,
                    Description="Игра Uncharted 4: A Thief’s End — это новая, четвертая часть популярного приключенческого экшена о Нейтане Дрейке, которая разрабатывается для PlayStation 4.\r\n\r\nНатан Дрейк возвращается с обновленной графикой, 1080p разрешением и совершенно новой историей. Благодаря мощи системы нового поколения PS4, модель Дрейка в Uncharted 4 намного превосходит модели из PS3-версий серии и имеет в два раза больше полигонов. На этот раз, Дрейк должен отыскать легендарное пиратское сокровище. Опасное путешествие подвергнет героя не только суровому физическому испытанию, но заставит его пожертвовать многим, чтобы спасти тех, кем он дорожит.\r\n\r\nНатан Дрейк нового поколения в A Thief’s End более реалистичен. Он вынужден будет вернуться в оставленный мир воров, отправившись в свое величайшее приключение, проверяя свои физические возможности и решимость. Он проверит себя, на что готов и чем готов пожертвовать, чтобы сохранить тех, кого он любит.",
                    ReleaseDate=DateTime.Parse("10 мая 2016")
                },
                new Game
                {
                    Name="The Witcher 3: Wild Hunt",
                    ImageSource="https://metarankings.ru/images/uploads/The-Witcher-3-Wild-Hunt-cover.png",
                    LocalizationId=1,
                    Score=8.7f,
                    Description="Игра The Witcher 3: Wild Hunt (Ведьмак 3: Дикая Охота) — новая часть в знаменитой серии от Польской студии CD Projekt RED, которая будет сочетать в себе фирменный  нелинейный сюжет и открытый разнообразный мир, который будет больше, чем любой другой в современной РПГ в купе с современной графикой. А общая площадь мира в тридцать раз больше, чем мир предыдущей части серии.\r\n\r\nВ Ведьмаке 3 вас ждет полностью открытый мир для исследования, без каких-либо искусственных границ и ограничений для его покорения. Для передвижения по этому многогранному и разнообразному миру можно будет использовать различный транспорт, например лошадей или корабли.\r\n\r\nСюжетная линия The Witcher 3: Wild Hunt продолжает историю Геральта из Ривии, ведьмака и охотника на монстров. В центре сюжета будет новое вторжение Нильфгаарда, а также поиски потерянной возлюбленной и конфликт с Дикой Охотой. Сюжет больше не будет делиться на разнообразные главы, что стало осуществимым благодаря движку нового поколения REDengine 3. А решения принятые по мере прохождения сюжетной компании будут влечь за собой более внушительные последствия, чем в предыдущих частях.",
                    ReleaseDate=DateTime.Parse("19 мая 2015")
                },
                new Game
                {
                    Name="God of War",
                    ImageSource="https://metarankings.ru/images/uploads/God-of-War-boxart-cover.jpg",
                    LocalizationId=1,
                    Score=9.0f,
                    Description="Игра God of War для PS4 — это перезапуск легендарной брутальной франшизы от Sony Santa Monica, который расскажет совершенно новую эмоциональную историю о путешествии Кратоса и даст игрокам переосмысленный геймплей с видом от третьего лица. Вы станете свидетелями убедительной драмы, которая разворачивается, когда бессмертные полубоги принимают решения о своей перемене.\r\n\r\nКратос решил измениться раз и навсегда, разорвать порочный круг бессмысленного насилия, который увековечил его падшую семью Олимпа. Теперь все былое в прошлом — злополучный контракт с Аресом, убийство его семьи и безумная ярость спровоцированная местью, которая в конечном итоге закончилась эпическим разрушением Олимпа. Теперь у Кратоса есть маленький сын за которого он несет ответственно и он обязан усмирить того монстра, который в нем живет и вырывается благодаря его ярости…",
                    ReleaseDate=DateTime.Parse("20 апреля 2018")
                },
                new Game
                {
                    Name="Grand Theft Auto V (обновленная версия)",
                    ImageSource="https://metarankings.ru/images/uploads/Grand-Theft-Auto-V-new-cover.png",
                    LocalizationId=2,
                    Score=8.2f,
                    Description="Grand Theft Auto V — это обновленная версия для консолей нового поколения и ПК вышедшей в прошлом году  пятой части легендарной серии.\r\n\r\nGTA5 обзавелась похорошевшей графикой, увеличенным разрешением, новыми деталями, большей дальностью прорисовки, всеми дополнениями и видом от первого лица. Теперь игроки могут исследовать мир Лос-Сантоса глазами своего персонажа от первого лица, открывая детали мира совершенно по новому.\r\n\r\nRockstar Games внесли массу всевозможных изменений, чтобы это стало возможным, добавив новую систему таргетинга, более традиционную схему управления для шутеров, а также тысячи новых анимаций в существующий игровой мир. Переключаться в вид от первого лица можно  по нажатию кнопки, так что вы можете легко переключаться между перспективами в реальном времени.",
                    ReleaseDate=DateTime.Parse("18 ноября 2014")
                },
                new Game
                {
                    Name="Red Dead Redemption 2",
                    ImageSource="https://metarankings.ru/images/uploads/red-dead-redemption-2-box-art-cover.jpg",
                    LocalizationId=2,
                    Score=8.4f,
                    Description="Игра Red Dead Redemption 2 — это новая глава в знаменитой серии о суровой жизни на Диком Западе от создателей Grand Theft Auto, студии Rockstar Games. Она рассказывает совершенно новую историю новых героев о жизни и выживании в самом сердце дикой Америки со всем из этого вытекающим — стволы, бандиты, шерифы, дикие животные и палящее солнце над головой. Вас ждет тщательно проработанный открытый мир, который так же станет основой для нового сетевого режима.",
                    ReleaseDate=DateTime.Parse("26 октября 2018")
                },
                new Game
                {
                    Name="Baldur’s Gate 3\r\n",
                    ImageSource="https://metarankings.ru/images/uploads/2020/09/baldurs-gate-3-boxart-cover.jpeg",
                    LocalizationId=2,
                    Score=8.3f,
                    Description="Игра Baldur’s Gate 3 — это третья часть легендарной ролевой серии, которую фанаты ждали долгое время. Древнее зло вновь возвращается ко Вратам Балдура, стремясь уничтожить все и вся изнутри, разрушая все на своем пути, что еще осталось в Забытых Королевствах. В одиночку вы можете сопротивляться этому злу, но вместе вы можете его победить.\r\n\r\nВам нужно собрать отряд и отправиться в Забытые Королевства, где вас ждет история о дружбе и предательстве, выживании и самопожертвовании, а также о привлекательной возможности обладать абсолютной властью. Однако ваш мозг заражен личинкой иллитида, которая дает вам странные и устрашающие способности. Вы можете противостоять паразиту и использовать свои силы против тьмы, или же погрузиться в зло и стать его инструментом.",
                    ReleaseDate=DateTime.Parse("3 августа 2023")
                },
                new Game
                {
                    Name="Grand Theft Auto V\r\n",
                    ImageSource="https://metarankings.ru/images/uploads/Grand-Theft-Auto-V-cover.png",
                    LocalizationId=2,
                    Score=8.4f,
                    Description="Игра Grand Theft Auto V (GTA 5) — последняя часть легендарной серии. В этой части сюжет вращается вокруг знакомого штата Сан-Андреас и города Лос-Сантос. Впервые за историю серии в GTA 5 присутствуют сразу три разных главных героя со своим характером и особенности, между которыми можно переключаться почти в любое время и месте. В некоторых миссиях доступно два игрока, в некоторых миссиях игрока перекидывает на других персонажей скриптом.\r\n\r\nВы узнаете историю сразу трех разный персонажей и побываете в их шкуре: Майкл — бывший грабитель банков, который возвращается к преступному образу жизни из-за своих финансовых проблем; Тревор Филлипс — друг Майкла, эмоциональный персонах, который страдает психологическим расстройством; Франклин — молодой чернокожий воришка, занимающийся выбиванием долгов для автосалона.",
                    ReleaseDate=DateTime.Parse("17 сентября 2013")
                },
                new Game
                {
                    Name="Red Dead Redemption\r\n",
                    ImageSource="https://metarankings.ru/images/uploads/Red-Dead-Redemption-boxart-cover.jpg",
                    LocalizationId=2,
                    Score=8.1f,
                    Description="События игры Red Dead Redemption развиваются в Америке начала XX века. Главный герой по имени Джон Марстон — бывший головорез, оставивший кровавый промысел в прошлом и вынужден работать на государственных агентов, чтобы убить, или захватить живыми его бывших подельников, включая одного «старого друга» с которым он совершал преступления — Билла Уилльямсона. Если Марстон этого не сделает, то его семья серьезно пострадает.\r\n\r\nВы отправитесь исследовать огромное пространство от западных границ США, где царит хаос и властвуют беспринципные по уши коррумпированные чиновники, а простые жители ведут бесконечную борьбу за своё выживание, посетите Мексику, пребывающую на грани гражданской войны, и доберетесь до цивилизованных городов американского севера, которые живут размеренной и спокойной жизнью.",
                    ReleaseDate=DateTime.Parse("28 мая 2010")
                },
                new Game
                {
                    Name="The Legend of Zelda: Breath of the Wild\r\n",
                    ImageSource="https://metarankings.ru/images/uploads/The-Legend-of-Zelda-Breath-of-the-Wild-boxart-cover.jpg",
                    LocalizationId=1,
                    Score=8.1f,
                    Description="Игра The Legend of Zelda: Breath of the Wild - это новая часть легендарной серии, которая разрабатывается эксклюзивно для консолей Nintendo — Wii U и Switch . Действия  развиваются в полностью открытом динамичном мире, а ее графика выходит на совершенно новый уровень. Игроки все также берут под свое управление эльфа по имени Линк, который обзавелся новыми умениями и навыками, благодаря новому геймпаду с новыми возможностями интерфейса. А мир игры все также наполнен головоломками, которые нужно будет решать.\r\n\r\nМожно забираться на башни и горные вершины в поисках новых целей, прокладывайте свои уникальные маршруты и почувствуйте дыхание дикой природы — можно собирать предметы, плавать, исследовать огромные пространства, карабкаться на высоты, рубить деревья, охотиться, сражаться с врагами, готовить зелья и так далее.",
                    ReleaseDate=DateTime.Parse("3 марта 2017")
                },
                new Game
                {
                    Name="Resident Evil 2",
                    ImageSource="https://metarankings.ru/images/uploads/resident-evil-2-box-art-cover.jpg",
                    LocalizationId=2,
                    Score=8.2f,
                    Description="Resident Evil 2 — это полноценный ремейк культовой игры, который разработан с полного нуля на современном графическом движке RE Engine. Как и оригинал, ремейк предложит две полноценные кампании — за полицейского Леона и студентку Клэр Редфилд, которая отчаянно ищет своего брата. Это проект с современным подходом к расположению камеры из-за плеча главного героя, вместо статического, как у оригинала.\r\n\r\nОригинальный RE2 увидел свет двадцать лет тому назад, в 1998 году. Как и в легендарном оригинале вы попадете в кишащий кровожадными зомби Raccoon City, в котором вас ждут ужасающие открытия, разнообразные головоломки, нелинейность сюжета и разнообразные локации!",
                    ReleaseDate=DateTime.Parse("25 января 2019")
                },
                new Game
                {
                    Name="Ведьмак 3: Дикая охота — Кровь и вино\r\n",
                    ImageSource="https://metarankings.ru/images/uploads/The-Witcher-3-Blood-and-Wine-boxart-cover.jpg",
                    LocalizationId=1,
                    Score=8.2f,
                    Description="Игра «Ведьмак 3: Дикая охота — Кровь и вино» — это второе и последнее крупное дополнение, после «Каменные Сердца» для успешного ролевого проекта Ведьмак 3: Дикая охота.\r\n\r\nКровь и вино — это последнее дополнение, которое дарит свыше 20 часов нового приключения на землях Туссент. Знакомые с книгами Анджея Сапковского вероятно слышали об этих землях и ждали возможности их исследовать в игровой вселенной. И вот, наконец у Вас будет возможность это сделать, отправившись в солнечный, красивый регион, который можно сравнить фэнтэзийными декорациями из сказок.",
                    ReleaseDate=DateTime.Parse("31 мая 2016")
                },
                new Game
                {
                    Name="Super Mario Odyssey\r\n",
                    ImageSource="https://metarankings.ru/images/uploads/bloodborne-box-art-cover.jpg",
                    LocalizationId=2,
                    Score=8.2f,
                    Description="Super Mario Odyssey — это масштабное трехмерное приключение знаменитого усатого водопроводчика по имени Марио. В этой игре он должен собирать луны, которые приводят в движение летательный корабль под названием «Одиссея», и помешайте Боузеру, который задумал жениться на принцессе Пич!\r\n\r\nЭто захватывающий платформер в открытом мире, который продолжает традиции, заложенные в игре Super Mario 64, вышедшей в 1997 году, и классической игре Sunshine для Nintendo GameCube от 2002 года. С такими новыми приемами Марио, как бросок шляпы, прыжок со шляпы и зашляпливание, вас ждет невероятно веселый и увлекательный гемплей, непохожий на другие игры с Марио. Приготовьтесь отравиться за границы Грибного королевства!",
                    ReleaseDate=DateTime.Parse("27 октября 2017")
                },
                new Game
                {
                    Name="Bloodborne",
                    ImageSource="https://metarankings.ru/images/uploads/bloodborne-box-art-cover.jpg",
                    LocalizationId=2,
                    Score=8.2f,
                    Description="Игра Bloodborne — это детище сотрудничества Japan Studio и создателей серий Demon/Dark Souls в жанре Экшен-РПГ от третьего лица в открытом мире, которое выходит эксклюзивно на PlayStation 4. Она переносит игроков в темный и зловещий готический мир, в мир, который наводнен непонятными существами, кошмарными созданиями и невероятными боссами. А гнетущая атмосфера делает ее страшной, пугающей и загадочной.\r\n\r\nДействия развиваются в древнем, забытом городе, который называется Яхарнам. На протяжении многих лет, многие безнадежные и угнетенные люди отправлялись в этот город в поисках помощи, а теперь же, этот город стал проклятым местом пораженным ужасным эндемическим заболеванием. Вы становитесь одним из этих людей, которые отравились в это место за помощью.",
                    ReleaseDate=DateTime.Parse("25 марта 2015")
                },
                new Game
                {
                    Name="Elden Ring\r\n",
                    ImageSource="https://metarankings.ru/images/uploads/2021/12/elden-ring.jpg",
                    LocalizationId=2,
                    Score=8.2f,
                    Description="Игра Bloodborne — это детище сотрудничества Japan Studio и создателей серий Demon/Dark Souls в жанре Экшен-РПГ от третьего лица в открытом мире, которое выходит эксклюзивно на PlayStation 4. Она переносит игроков в темный и зловещий готический мир, в мир, который наводнен непонятными существами, кошмарными созданиями и невероятными боссами. А гнетущая атмосфера делает ее страшной, пугающей и загадочной.\r\n\r\nДействия развиваются в древнем, забытом городе, который называется Яхарнам. На протяжении многих лет, многие безнадежные и угнетенные люди отправлялись в этот город в поисках помощи, а теперь же, этот город стал проклятым местом пораженным ужасным эндемическим заболеванием. Вы становитесь одним из этих людей, которые отравились в это место за помощью.",
                    ReleaseDate=DateTime.Parse("25 марта 2015")
                },
                new Game
                {
                    Name="The Last of Us Part II",
                    ImageSource="https://metarankings.ru/images/uploads/the-last-of-us-part-2-box-art-cover.jpg",
                    LocalizationId=1,
                    Score=7.6f,
                    Description="The Last of Us Part II (Одни из нас. Часть II) — это продолжение культовой игры от студии Naughty Dog, которая получила множество наград и любовь игроков по всему миру. Полюбившиеся многими персонажи Элли и Джоэля возвращаются в еще более захватывающем, душераздирающем и эмоциональном приключении. Помимо Элли и Джоэля нас ждут и совершенно новые персонажи, которые так же выживают в суровых реалиях пост-апокалиптического мира и являются неотъемлемой частью TLoU Part II.\r\n\r\nЭлли повзрослела и выходит на тропу войны со своими кровными врагами, а Джоэл пытается ей разобраться с собой и с ее проблемами… В «Одни из нас 2» вас ждет трогательная история, затрагивающая очень глубокие темы человеческого бытия. Это самая амбициозный проект студии Naughty Dog, которая подарит незабываемые эмоции всем поклонникам оригинала и новым игрокам, которые впервые будут знакомить с постапокалиптическим миром наводненном щелкунами, и кровожадными людьми, способными на все ради выживания…\r\n\r\nПосле смертельно опасного путешествия из первой части «Одних из нас», которая простиралась по охваченной страшной эпидемией Америке Элли и Джоэл наконец осели в Вайоминге. Обосновавшись в процветающей общине, они наконец обрели желанную стабильность, несмотря на постоянную угрозу нападения зараженных и мародеров. Но и стабильности рано или поздно приходит конец…",
                    ReleaseDate=DateTime.Parse("19 июня 2020")
                }
                ]);

            await dataContext.SaveChangesAsync();
        }

        var games = await dataContext.Games
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(g => g.Genres)
            .Include(g => g.CriticsReviews)
            .Include(g => g.Developers)
            .Include(g => g.Platforms)
            .Include(g => g.Localization)
            .Include(g => g.Tags)
            .Include(g => g.Publishers)
            .Include(g => g.UsersReviews)
            .Include(g => g.Trailers).ToArrayAsync();
        return Ok(games);
    }
}
