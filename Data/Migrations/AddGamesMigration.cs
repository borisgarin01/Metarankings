using FluentMigrator;

namespace Data.Migrations;

[Migration(16, "Add games migration")]
public sealed class AddGamesMigration : Migration
{
    public override void Down()
    {
		Execute.Sql(@"DELETE FROM Games 
WHERE Name in ('The Last of Us: Remastered', 'God of War (2018)', 'The Witcher 3: Wild Hunt', 'Grand Theft Auto V (обновленная версия)');");
    }

    public override void Up()
    {
		Execute.Sql(@"
insert into games(href, name, image, localizationid, publisherid, releasedate, description, trailer)
values
	('games/the-last-of-us-remastered', 
	'The Last of Us: Remastered', 
	'images/uploads/2014/07/the-last-of-us-remastered-boxart-cover.jpg',
	(SELECT id FROM localizations WHERE name = 'Полностью на русском языке' LIMIT 1),
    (SELECT id FROM publishers WHERE name = 'Sony Computer Entertainment' LIMIT 1),
	'2014-07-30',
	'The Last of Us: Remastered — это переиздание лучшей игры 2013 для PlayStation 3 — The Last of Us, которое будет иметь более большее разрешение 1080p, модели более высокого разрешения, новые тени и освещение, обновленные текстуры и множество других улучшений.

Сюжет The Last of Us: Remastered отправит вас в пост-апокалиптический зараженный мир, где люди превращаются в зомби-мутантов, но не они ваш главный враг, а выжившие люди.

В обновленную версии The Last of Us для PS4 также войдут внутриигровое видео с комментариями от Нила Дракманна (сценарист, арт-директор игры) и тонна дополнительного контента из оригинала.',
'https://youtu.be/DmP2QLKJFr0'
	),
	('games/god-of-war-2018', 
	'God of War (2018)', 
	'images/uploads/2018/04/God-of-War-boxart-cover.jpg',
	(SELECT id FROM localizations WHERE name = 'Полностью на русском языке' LIMIT 1),
    (SELECT id FROM publishers WHERE name = 'Sony Computer Entertainment' LIMIT 1),
	'2018-04-20',
	'Игра God of War для PS4 — это перезапуск легендарной брутальной франшизы от Sony Santa Monica, который расскажет совершенно новую эмоциональную историю о путешествии Кратоса и даст игрокам переосмысленный геймплей с видом от третьего лица. Вы станете свидетелями убедительной драмы, которая разворачивается, когда бессмертные полубоги принимают решения о своей перемене.

В новом God of War Кратос решил измениться раз и навсегда, разорвать порочный круг бессмысленного насилия, который увековечил его падшую семью Олимпа. Теперь все былое в прошлом — злополучный контракт с Аресом, убийство его семьи и безумная ярость спровоцированная местью, которая в конечном итоге закончилась эпическим разрушением Олимпа. Теперь у Кратоса есть маленький сын за которого он несет ответственно и он обязан усмирить того монстра, который в нем живет и вырывается благодаря его ярости…',
'https://youtu.be/lRhzFqA1f7o'
	),
('games/the-witcher-3-wild-hunt', 
	'The Witcher 3: Wild Hunt', 
	'images/uploads/2015/05/The-Witcher-3-Wild-Hunt-cover.png',
	(SELECT id FROM localizations WHERE name = 'Полностью на русском языке' LIMIT 1),
    (SELECT id FROM publishers WHERE name = 'Namco Bandai Games' LIMIT 1),
	'2015-05-19',
	'Игра The Witcher 3: Wild Hunt (Ведьмак 3: Дикая Охота) — новая часть в знаменитой серии от Польской студии CD Projekt RED, которая будет сочетать в себе фирменный  нелинейный сюжет и открытый разнообразный мир, который будет больше, чем любой другой в современной РПГ в купе с современной графикой. А общая площадь мира в тридцать раз больше, чем мир предыдущей части серии.

В Ведьмаке 3 вас ждет полностью открытый мир для исследования, без каких-либо искусственных границ и ограничений для его покорения. Для передвижения по этому многогранному и разнообразному миру можно будет использовать различный транспорт, например лошадей или корабли.

Сюжетная линия The Witcher 3: Wild Hunt продолжает историю Геральта из Ривии, ведьмака и охотника на монстров. В центре сюжета будет новое вторжение Нильфгаарда, а также поиски потерянной возлюбленной и конфликт с Дикой Охотой. Сюжет больше не будет делиться на разнообразные главы, что стало осуществимым благодаря движку нового поколения REDengine 3. А решения принятые по мере прохождения сюжетной компании будут влечь за собой более внушительные последствия, чем в предыдущих частях.',
'https://youtu.be/HtVdAasjOgUo'
	),
('games/grand-theft-auto-v-upgraded-version',
'Grand Theft Auto V (обновленная версия)',
'images/uploads/2014/11/Grand-Theft-Auto-V-new-cover.png',
(SELECT id FROM localizations WHERE name = 'Субтитры на русском языке' LIMIT 1),
(SELECT id FROM publishers WHERE name = 'Rockstar Games' LIMIT 1),
'2014-11-18',
'Grand Theft Auto V — это обновленная версия для консолей нового поколения и ПК вышедшей в прошлом году  пятой части легендарной серии.

GTA5 обзавелась похорошевшей графикой, увеличенным разрешением, новыми деталями, большей дальностью прорисовки, всеми дополнениями и видом от первого лица. Теперь игроки могут исследовать мир Лос-Сантоса глазами своего персонажа от первого лица, открывая детали мира совершенно по новому.

Rockstar Games внесли массу всевозможных изменений, чтобы это стало возможным, добавив новую систему таргетинга, более традиционную схему управления для шутеров, а также тысячи новых анимаций в существующий игровой мир. Переключаться в вид от первого лица можно  по нажатию кнопки, так что вы можете легко переключаться между перспективами в реальном времени.',
'https://youtu.be/qR4pdI5cmUc');

	insert into gamesplatforms(gameid,platformid) values
	((select id from games where name = 'The Last of Us: Remastered'), (select id from platforms where name='PS4')),
	((select id from games where name = 'God of War (2018)'), (select id from platforms where name='PS4')),
	((select id from games where name = 'The Witcher 3: Wild Hunt'), (select id from platforms where name='PC')),
	((select id from games where name = 'The Witcher 3: Wild Hunt'), (select id from platforms where name='PS4')),
	((select id from games where name = 'The Witcher 3: Wild Hunt'), (select id from platforms where name='Switch')),
	((select id from games where name = 'The Witcher 3: Wild Hunt'), (select id from platforms where name='Xbox One')),
	((select id from games where name = 'Grand Theft Auto V (обновленная версия)'), (select id from platforms where name='PC')),
	((select id from games where name = 'Grand Theft Auto V (обновленная версия)'), (select id from platforms where name='PS4')),
	((select id from games where name = 'Grand Theft Auto V (обновленная версия)'), (select id from platforms where name='Xbox One'));

	insert into gamesdevelopers(gameid, developerid) values
	((select id from games where name = 'The Last of Us: Remastered'), (select id from developers where name='Naughty Dog')),
	((select id from games where name = 'God of War (2018)'), (select id from developers where name='Sony Santa Monica')),
	((select id from games where name = 'The Witcher 3: Wild Hunt'), (select id from developers where name='CD Projekt RED')),
	((select id from games where name = 'Grand Theft Auto V (обновленная версия)'), (select id from developers where name='Rockstar Games'));

	insert into gamesgenres(gameid, genreid) values
	((select id from games where name = 'The Last of Us: Remastered'), (select id from genres where name = 'Приключение')),
	((select id from games where name = 'The Last of Us: Remastered'), (select id from genres where name = 'Хоррор')),
	((select id from games where name = 'The Last of Us: Remastered'), (select id from genres where name = 'Экшен')),
	((select id from games where name = 'God of War (2018)'), (select id from genres where name = 'Приключение')),
	((select id from games where name = 'God of War (2018)'), (select id from genres where name = 'Экшен')),
	((select id from games where name = 'The Witcher 3: Wild Hunt'), (select id from genres where name = 'РПГ')),
	((select id from games where name = 'Grand Theft Auto V (обновленная версия)'), (select id from genres where name = 'Приключение')),
	((select id from games where name = 'Grand Theft Auto V (обновленная версия)'), (select id from genres where name = 'Экшен'));
;");
    }
}
