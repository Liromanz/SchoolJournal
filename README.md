# SchoolJournal
Электронный журнал для техникума без подключенной базы данных (локальная сериализация вместо БД)

В электронном журнале можно регистрироваться тремя способами: студент, преподаватель и администратор
Возможности студента:
  - Просматривать расписание за день
  - Просматривать выставленные оценки за предмет.

Возможности преподавателя:
  - Выбирать группу, дисциплину и день для выставления отметки для занятия
  - Просматривать, за какие пары выставлена оценка, а за какие нет.

Возможности администратора:
  - Регистрировать новых пользователей, менять данные у текущих
  - Добавлять новые группы студентов
  - Добавлять новые дисциплины
  - Изменять расписание, указывать, какой преподаватель к какой дисциплине относится.

При регистрации студента появится выпадающий список с группой. Он необязательный для заполнения,
если групп студента нет, администратор сможет изменить данные о группе студента позже
Для регистрации администратора следует 8 раз нажать по кнопке "Регистрация" в главном меню.
После этого в поле "Должность" в регистрации появится пункт "Администратор"

Для полного изучения программы стоит сначала зарегистрироваться за администратора, заполнить
данные о группе и дисциплине и создать маленькое расписание для группы. Затем зарегистрироваться за
преподавателя и выставить оценки за некоторые дисциплины (дисциплина выбирается по дате). 
В конце - зарегистрироваться за студента и посмотреть выставленные оценки
