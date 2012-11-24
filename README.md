TODOListDemo
============
Basic TODO List Demonstration application.

Functionalities:
- multi-user system with registration and email confirmation
- adding, editing, viewing, archiving and deleting tasks
- tasks prioritised in 3 levels: high, medium, low
- user enabled sorting of tasks with drag and drop functionalities (exception: tasks whose time has passed and newly added tasks always show on top of preferred user order or default order, default ordering: highest priority first then start time)
- view of archived (done) and active tasks
- show more button on TODO List (default view shows 20 tasks)
- Ajax loading of changes on page

Whole project was made in Microsoft Visual Web Developer Express 2010. (http://www.microsoft.com/visualstudio/eng/downloads#d-2010-express)

Technologies used: 
- ASP.Net MVC 3 (http://www.asp.net/mvc/mvc3)
- Ninject (http://www.ninject.org/)
- Twitter Bootstrap (http://twitter.github.com/bootstrap/)
- FullCalendar jQuery plugin (http://arshaw.com/fullcalendar/)
- jQuery (http://jquery.com/)
- CSS

Tested with: 
- Google Chrome version 22.0.1229.94
- Firefox version 16.0.2.
- Opera version 12.02
- Internet Explorer version 9.0.8112.16421
- Safari version 5.1.7

Fixed bugs:
- In Internet Explorer popup dialogs do not work correctly. They show up, but they disappear with click within a browser so forms cannot be filled. (fix date: 30/10/2012)

Preview of app:
 - default view: http://i48.tinypic.com/153wqw8.jpg
 - edit task: http://oi45.tinypic.com/64p54k.jpg
 - task details: http://i49.tinypic.com/9k72hg.png
 - archived tasks view: http://i48.tinypic.com/25j9g21.png