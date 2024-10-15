Перейдите в папку проекта:

1. Откройте проводник файлов (File Explorer).
2. Перейдите в распакованную папку проекта.
3. Откройте командную строку (Command Prompt):
4. В адресной строке проводника в папке проекта введите cmd и нажмите Enter. Это откроет командную строку прямо в нужной папке.

Запуск проекта:
1. В командной строке выполните команду:
dotnet restore
2. После этого запустите проект командой:
dotnet run

Откроется Swagger для тестирования API:
  После успешного запуска проекта в командной строке появится адрес, например: http://localhost:5000 или http://localhost:7030.
  Откройте этот адрес в вашем веб-браузере, добавив /swagger:
  http://localhost:5000/swagger
В Swagger вы сможете увидеть и протестировать все доступные API-методы.
  В Swagger выберите нужный метод API.
  Нажмите на кнопку Try it out.
  Введите необходимые параметры и нажмите Execute.
  Результаты запроса и ответ сервера отобразятся ниже.