using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("{title}")]
    public async Task<ActionResult<DetailsComponentItem>> GetDetailsComponentItemsAsync(string title)
    {
#if DEBUG
        dataContext.DetailsComponentsItems.AddRange(new DetailsComponentItem[]
        {
                new DetailsComponentItem
                {
                    CriticsAverageScore=10,
                    Description="Это переиздание лучшей игры 2013 для PlayStation 3 — The Last of Us, которое будет иметь более большее разрешение 1080p, модели более высокого разрешения, новые тени и освещение, обновленные текстуры и множество других улучшений.Сюжет игры отправит вас в пост-апокалиптический зараженный мир, где люди превращаются в зомби-мутантов, но не они ваш главный враг, а выжившие люди.В обновленную версии для PS4 также войдут внутриигровое видео с комментариями от Нила Дракманна (сценарист, арт-директор игры) и тонна дополнительного контента из оригинала.",
                    ExpectationsPercent=2,
                    GamersAverageScore=8.8f,
                    ImageSource="https://metarankings.ru/images/uploads/2014/07/the-last-of-us-remastered-boxart-cover.jpg",
                    MarksCount=2038,
                    Metarating=9.4f,
                    Name="The Last of Us: Remastered",
                    OriginalName="The Last of Us: Remastered",
                    PremiereDate=new DateOnly(2014,07,30)
                },
                new DetailsComponentItem
                {
                    CriticsAverageScore=9.7f,
                    Description="Игра Uncharted 4: A Thief’s End — это новая, четвертая часть популярного приключенческого экшена о Нейтане Дрейке, которая разрабатывается для PlayStation 4.Натан Дрейк возвращается с обновленной графикой, 1080p разрешением и совершенно новой историей. Благодаря мощи системы нового поколения PS4, модель Дрейка в Uncharted 4 намного превосходит модели из PS3-версий серии и имеет в два раза больше полигонов. На этот раз, Дрейк должен отыскать легендарное пиратское сокровище. Опасное путешествие подвергнет героя не только суровому физическому испытанию, но заставит его пожертвовать многим, чтобы спасти тех, кем он дорожит.Натан Дрейк нового поколения в A Thief’s End более реалистичен. Он вынужден будет вернуться в оставленный мир воров, отправившись в свое величайшее приключение, проверяя свои физические возможности и решимость. Он проверит себя, на что готов и чем готов пожертвовать, чтобы сохранить тех, кого он любит.",
                    ExpectationsPercent=89,
                    GamersAverageScore=8.7f,
                    ImageSource="https://metarankings.ru/images/uploads/2016/05/uncharted-4-a-thiefs-end-boxart-cover.jpg",
                    MarksCount=1898,
                    Metarating=9.2f,
                    Name="Uncharted 4: A Thief’s End",
                    OriginalName="Uncharted 4: A Thief’s End",
                    PremiereDate=new DateOnly(2016,5,10)
                },
                new DetailsComponentItem
                {
                    CriticsAverageScore=9.4f,
                    Description="The Last of Us — приключенческая экшн игра от создателей серии Uncharted, студии Naughty Dog. События The Last of Us развиваются спустя 20 лет после начала страшной эпидемии, которая превращает всех людей в зомби-мутантов. Джоел вместе с Элли отправляется в сложное путешествие через страну, бывшую некогда великой, но ставшей лишь заросшими зеленью руинами с жалкими остатками выживающих. Вам не раз предстоит с ними столкнуться и посмотреть на их жизнь в условиях страшного апокалипсиса. Девочка Элли будет всячески помогать вам на протяжении всего прохождения . Придется создавать предметы и использовать аптечки. Это эмоциональное произведение, где основным противником выступают сами люди, их нравы и совесть, а не зараженные.",
                    ExpectationsPercent=2,
                    GamersAverageScore=9f,
                    ImageSource="https://metarankings.ru/images/uploads/2013/06/the-last-of-us-boxart-cover.jpg",
                    MarksCount=1898,
                    Metarating=9.2f,
                    Name="The Last of Us (Одни из нас)",
                    OriginalName="The Last of Us (Одни из нас)",
                    PremiereDate=new DateOnly(2013,06,14)
                },
                new DetailsComponentItem
                {
                    CriticsAverageScore=9.6f,
                    Description="Игра The Witcher 3: Wild Hunt (Ведьмак 3: Дикая Охота) — новая часть в знаменитой серии от Польской студии CD Projekt RED, которая будет сочетать в себе фирменный  нелинейный сюжет и открытый разнообразный мир, который будет больше, чем любой другой в современной РПГ в купе с современной графикой. А общая площадь мира в тридцать раз больше, чем мир предыдущей части серии. В Ведьмаке 3 вас ждет полностью открытый мир для исследования, без каких-либо искусственных границ и ограничений для его покорения. Для передвижения по этому многогранному и разнообразному миру можно будет использовать различный транспорт, например лошадей или корабли. Сюжетная линия The Witcher 3: Wild Hunt продолжает историю Геральта из Ривии, ведьмака и охотника на монстров. В центре сюжета будет новое вторжение Нильфгаарда, а также поиски потерянной возлюбленной и конфликт с Дикой Охотой. Сюжет больше не будет делиться на разнообразные главы, что стало осуществимым благодаря движку нового поколения REDengine 3. А решения принятые по мере прохождения сюжетной компании будут влечь за собой более внушительные последствия, чем в предыдущих частях.",
                    ExpectationsPercent=98,
                    GamersAverageScore=8.7f,
                    ImageSource="https://metarankings.ru/images/uploads/The-Witcher-3-Wild-Hunt-cover.png",
                    MarksCount=3055,
                    Metarating=9.2f,
                    Name="The Witcher 3: Wild Hunt",
                    OriginalName="The Witcher 3: Wild Hunt",
                    PremiereDate=new DateOnly(2015,5,19)
                },
                new DetailsComponentItem
                {
                    CriticsAverageScore=9.3f,
                    Description="Игра God of War для PS4 — это перезапуск легендарной брутальной франшизы от Sony Santa Monica, который расскажет совершенно новую эмоциональную историю о путешествии Кратоса и даст игрокам переосмысленный геймплей с видом от третьего лица. Вы станете свидетелями убедительной драмы, которая разворачивается, когда бессмертные полубоги принимают решения о своей перемене.\r\n\r\nКратос решил измениться раз и навсегда, разорвать порочный круг бессмысленного насилия, который увековечил его падшую семью Олимпа. Теперь все былое в прошлом — злополучный контракт с Аресом, убийство его семьи и безумная ярость спровоцированная местью, которая в конечном итоге закончилась эпическим разрушением Олимпа. Теперь у Кратоса есть маленький сын за которого он несет ответственно и он обязан усмирить того монстра, который в нем живет и вырывается благодаря его ярости…",
                    ExpectationsPercent=89,
                    GamersAverageScore=9f,
                    ImageSource="https://metarankings.ru/images/uploads/The-Witcher-3-Wild-Hunt-cover.png",
                    MarksCount=1824,
                    Metarating=9.2f,
                    Name="God of War",
                    OriginalName="God of War",
                    PremiereDate=new DateOnly(2018,4,20)
                },
                new DetailsComponentItem
                {
                    CriticsAverageScore=10f,
                    Description="Grand Theft Auto V — это обновленная версия для консолей нового поколения и ПК вышедшей в прошлом году  пятой части легендарной серии.\r\n\r\nGTA5 обзавелась похорошевшей графикой, увеличенным разрешением, новыми деталями, большей дальностью прорисовки, всеми дополнениями и видом от первого лица. Теперь игроки могут исследовать мир Лос-Сантоса глазами своего персонажа от первого лица, открывая детали мира совершенно по новому.\r\n\r\nRockstar Games внесли массу всевозможных изменений, чтобы это стало возможным, добавив новую систему таргетинга, более традиционную схему управления для шутеров, а также тысячи новых анимаций в существующий игровой мир. Переключаться в вид от первого лица можно  по нажатию кнопки, так что вы можете легко переключаться между перспективами в реальном времени.",
                    ExpectationsPercent=2,
                    GamersAverageScore=8.2f,
                    ImageSource="https://metarankings.ru/images/uploads/Grand-Theft-Auto-V-new-cover.png",
                    MarksCount=1627,
                    Metarating=9.1f,
                    Name="Grand Theft Auto V (обновленная версия)\r\n",
                    OriginalName="Grand Theft Auto V (обновленная версия)\r\n",
                    PremiereDate=new DateOnly(2014,11,18)
                },
                new DetailsComponentItem
                {
                    CriticsAverageScore=9.7f,
                    Description="Игра Red Dead Redemption 2 — это новая глава в знаменитой серии о суровой жизни на Диком Западе от создателей Grand Theft Auto, студии Rockstar Games. Она рассказывает совершенно новую историю новых героев о жизни и выживании в самом сердце дикой Америки со всем из этого вытекающим — стволы, бандиты, шерифы, дикие животные и палящее солнце над головой. Вас ждет тщательно проработанный открытый мир, который так же станет основой для нового сетевого режима.",
                    ExpectationsPercent=85,
                    GamersAverageScore=8.4f,
                    ImageSource="https://metarankings.ru/images/uploads/red-dead-redemption-2-box-art-cover.jpg",
                    MarksCount=1627,
                    Metarating=9.1f,
                    Name="Red Dead Redemption 2\r\n",
                    OriginalName="Red Dead Redemption 2\r\n",
                    PremiereDate=new DateOnly(2014,10,26)
                },
                new DetailsComponentItem
                {
                    CriticsAverageScore=9.4f,
                    Description="Игра Grand Theft Auto V (GTA 5) — последняя часть легендарной серии. В этой части сюжет вращается вокруг знакомого штата Сан-Андреас и города Лос-Сантос. Впервые за историю серии в GTA 5 присутствуют сразу три разных главных героя со своим характером и особенности, между которыми можно переключаться почти в любое время и месте. В некоторых миссиях доступно два игрока, в некоторых миссиях игрока перекидывает на других персонажей скриптом.\r\n\r\nВы узнаете историю сразу трех разный персонажей и побываете в их шкуре: Майкл — бывший грабитель банков, который возвращается к преступному образу жизни из-за своих финансовых проблем; Тревор Филлипс — друг Майкла, эмоциональный персонах, который страдает психологическим расстройством; Франклин — молодой чернокожий воришка, занимающийся выбиванием долгов для автосалона.",
                    ExpectationsPercent=2,
                    GamersAverageScore=8.4f,
                    ImageSource="https://metarankings.ru/images/uploads/Grand-Theft-Auto-V-cover.png",
                    MarksCount=1005,
                    Metarating=8.9f,
                    Name="Grand Theft Auto V\r\n",
                    OriginalName="Grand Theft Auto V\r\n",
                    PremiereDate=new DateOnly(2013,9,17)
                },
                new DetailsComponentItem
                {
                    CriticsAverageScore=9.5f,
                    Description="Игра Baldur’s Gate 3 — это третья часть легендарной ролевой серии, которую фанаты ждали долгое время. Древнее зло вновь возвращается ко Вратам Балдура, стремясь уничтожить все и вся изнутри, разрушая все на своем пути, что еще осталось в Забытых Королевствах. В одиночку вы можете сопротивляться этому злу, но вместе вы можете его победить.\r\n\r\nВам нужно собрать отряд и отправиться в Забытые Королевства, где вас ждет история о дружбе и предательстве, выживании и самопожертвовании, а также о привлекательной возможности обладать абсолютной властью. Однако ваш мозг заражен личинкой иллитида, которая дает вам странные и устрашающие способности. Вы можете противостоять паразиту и использовать свои силы против тьмы, или же погрузиться в зло и стать его инструментом.",
                    ExpectationsPercent=72,
                    GamersAverageScore=8.3f,
                    ImageSource="https://metarankings.ru/images/uploads/2020/09/baldurs-gate-3-boxart-cover.jpeg",
                    MarksCount=202,
                    Metarating=8.9f,
                    Name="Baldur’s Gate 3\r\n",
                    OriginalName="Baldur’s Gate 3\r\n",
                    PremiereDate=new DateOnly(2023,8,3)
                },
                new DetailsComponentItem
                {
                    CriticsAverageScore=9.5f,
                    Description="События игры Red Dead Redemption развиваются в Америке начала XX века. Главный герой по имени Джон Марстон — бывший головорез, оставивший кровавый промысел в прошлом и вынужден работать на государственных агентов, чтобы убить, или захватить живыми его бывших подельников, включая одного «старого друга» с которым он совершал преступления — Билла Уилльямсона. Если Марстон этого не сделает, то его семья серьезно пострадает.\r\n\r\nВы отправитесь исследовать огромное пространство от западных границ США, где царит хаос и властвуют беспринципные по уши коррумпированные чиновники, а простые жители ведут бесконечную борьбу за своё выживание, посетите Мексику, пребывающую на грани гражданской войны, и доберетесь до цивилизованных городов американского севера, которые живут размеренной и спокойной жизнью.",
                    ExpectationsPercent=72,
                    GamersAverageScore=8.8f,
                    ImageSource="https://metarankings.ru/images/uploads/Red-Dead-Redemption-boxart-cover.jpg",
                    MarksCount=213,
                    Metarating=8.8f,
                    Name="Red Dead Redemption\r\n",
                    OriginalName="Red Dead Redemption\r\n",
                    PremiereDate=new DateOnly(2010,5,28)
                },
                new DetailsComponentItem
                {
                    CriticsAverageScore=9.5f,
                    Description="Игра The Legend of Zelda: Breath of the Wild - это новая часть легендарной серии, которая разрабатывается эксклюзивно для консолей Nintendo — Wii U и Switch . Действия  развиваются в полностью открытом динамичном мире, а ее графика выходит на совершенно новый уровень. Игроки все также берут под свое управление эльфа по имени Линк, который обзавелся новыми умениями и навыками, благодаря новому геймпаду с новыми возможностями интерфейса. А мир игры все также наполнен головоломками, которые нужно будет решать.\r\n\r\nМожно забираться на башни и горные вершины в поисках новых целей, прокладывайте свои уникальные маршруты и почувствуйте дыхание дикой природы — можно собирать предметы, плавать, исследовать огромные пространства, карабкаться на высоты, рубить деревья, охотиться, сражаться с врагами, готовить зелья и так далее.",
                    ExpectationsPercent=77,
                    GamersAverageScore=8.1f,
                    ImageSource="https://metarankings.ru/images/uploads/The-Legend-of-Zelda-Breath-of-the-Wild-boxart-cover.jpg",
                    MarksCount=453,
                    Metarating=8.8f,
                    Name="The Legend of Zelda: Breath of the Wild\r\n",
                    OriginalName="The Legend of Zelda: Breath of the Wild\r\n",
                    PremiereDate=new DateOnly(2017,3,3)
                },
                new DetailsComponentItem
                {
                    CriticsAverageScore=9.3f,
                    Description="Resident Evil 2 — это полноценный ремейк культовой игры, который разработан с полного нуля на современном графическом движке RE Engine. Как и оригинал, ремейк предложит две полноценные кампании — за полицейского Леона и студентку Клэр Редфилд, которая отчаянно ищет своего брата. Это проект с современным подходом к расположению камеры из-за плеча главного героя, вместо статического, как у оригинала.\r\n\r\nОригинальный RE2 увидел свет двадцать лет тому назад, в 1998 году. Как и в легендарном оригинале вы попадете в кишащий кровожадными зомби Raccoon City, в котором вас ждут ужасающие открытия, разнообразные головоломки, нелинейность сюжета и разнообразные локации!",
                    ExpectationsPercent=80,
                    GamersAverageScore=8.2f,
                    ImageSource="https://metarankings.ru/images/uploads/resident-evil-2-box-art-cover.jpg",
                    MarksCount=453,
                    Metarating=8.8f,
                    Name="Resident Evil 2\r\n",
                    OriginalName="Resident Evil 2\r\n",
                    PremiereDate=new DateOnly(2019,1,25)
                },
                new DetailsComponentItem
                {
                    CriticsAverageScore=9.3f,
                    Description="Игра «Ведьмак 3: Дикая охота — Кровь и вино» — это второе и последнее крупное дополнение, после «Каменные Сердца» для успешного ролевого проекта Ведьмак 3: Дикая охота.\r\n\r\nКровь и вино — это последнее дополнение, которое дарит свыше 20 часов нового приключения на землях Туссент. Знакомые с книгами Анджея Сапковского вероятно слышали об этих землях и ждали возможности их исследовать в игровой вселенной. И вот, наконец у Вас будет возможность это сделать, отправившись в солнечный, красивый регион, который можно сравнить фэнтэзийными декорациями из сказок.",
                    ExpectationsPercent=87,
                    GamersAverageScore=8.2f,
                    ImageSource="https://metarankings.ru/images/uploads/The-Witcher-3-Blood-and-Wine-boxart-cover.jpg",
                    MarksCount=797,
                    Metarating=8.8f,
                    Name="Ведьмак 3: Дикая охота — Кровь и вино\r\n",
                    OriginalName="Ведьмак 3: Дикая охота — Кровь и вино\r\n",
                    PremiereDate=new DateOnly(2016,5,31)
                },

                new DetailsComponentItem
                {
                    CriticsAverageScore=9.7f,
                    Description="Super Mario Odyssey — это масштабное трехмерное приключение знаменитого усатого водопроводчика по имени Марио. В этой игре он должен собирать луны, которые приводят в движение летательный корабль под названием «Одиссея», и помешайте Боузеру, который задумал жениться на принцессе Пич!\r\n\r\nЭто захватывающий платформер в открытом мире, который продолжает традиции, заложенные в игре Super Mario 64, вышедшей в 1997 году, и классической игре Sunshine для Nintendo GameCube от 2002 года. С такими новыми приемами Марио, как бросок шляпы, прыжок со шляпы и зашляпливание, вас ждет невероятно веселый и увлекательный гемплей, непохожий на другие игры с Марио. Приготовьтесь отравиться за границы Грибного королевства!",
                    ExpectationsPercent=1,
                    GamersAverageScore=7.8f,
                    ImageSource="https://metarankings.ru/images/uploads/The-Witcher-3-Blood-and-Wine-boxart-cover.jpg",
                    MarksCount=797,
                    Metarating=8.8f,
                    Name="Super Mario Odyssey\r\n",
                    OriginalName="Super Mario Odyssey\r\n",
                    PremiereDate=new DateOnly(2017,10,27)
                }
        });
#endif
        await dataContext.SaveChangesAsync();
        return Ok();
    }
}
