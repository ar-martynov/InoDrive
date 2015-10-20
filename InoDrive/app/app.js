var App = App ||
{
    apiClientId: 'testQueries',
    apiClientSecret: '123@abc',
    apiPath: '/api',

    debug: true,
    session: null,
    router: null,
    locale: "Ru",

    Hubs: {},
    Models: {},
    Collections: {},
    Views: {
        CurrentView: null,
        CurrentModalView: null,

        Dispose: function () {
            if (App.Views.CurrentView != null) {
                App.Views.CurrentView.close();
                $.ajax().abort();
            }
        },

        DisposeModal: function () {
            if (App.Views.CurrentModalView != null) App.Views.CurrentModalView.close();
        },
    },

    Init: function () {

        this.Alerts.Init();
        this.debug ? $.connection.hub.logging = true : $.connection.hub.logging = false;

        this.session = new App.Models.Session();

        this.router = new App.Router();
        Backbone.history.start({ silent: false });


    },

    Alerts: {
        AlertMessages: {},
        Init: function () {
            this.AlertMessages["Authorization"] = "Авторизуйтесь для дальнейшей работы в системе.";
            this.AlertMessages["BidRejected"] = "Заявка отозвана.";
            this.AlertMessages["Rated"] = "Спасибо, ваш голос учтен!";
            this.AlertMessages["DestExist"] = "Указанный пункт является пунктом назначения!";
            this.AlertMessages["DestIsWaypoint"] = "Пункт назначения уже добавлен в качестве промежуточного!";
            this.AlertMessages["FileNotImage"] = "Файл не является изображением.";
            this.AlertMessages["FileSizeError"] = "Размер файла превышает 2Mb.";
            this.AlertMessages["ImageSizeError"] = "Допустимый размер изображения: 250x250.",
            this.AlertMessages["LogIn"] = "Вы успешно вошли в систему!";
            this.AlertMessages["LogOut"] = "Вы вышли из системы.";
            this.AlertMessages["NotAuthorized"] = "Для данного действия требуется авторизация!";
            this.AlertMessages["PathNotFound"] = "Системе не удается найти указаный путь!";
            this.AlertMessages["ProfileUpdated"] = "Вы успешно вошли в систему!";
            this.AlertMessages["PointExist"] = "Данный пункт уже добавлен в маршрут!";
            this.AlertMessages["SessionRefreshError"] = "Ошибка продления сессии.";
            this.AlertMessages["StartExist"] = "Указанный пункт является пунктом отправления!";
            this.AlertMessages["StartIsWaypoint"] = "Пункт отправления уже добавлен в качестве промежуточного!";
            this.AlertMessages["StartDestRequired"] = "Укажите сначала пункты отправления и назначения!";
            this.AlertMessages["TourCreated"] = "Поездка создана!";
        },
        showAlert: function (text, type, isServerMessage, options) {
            if (isServerMessage) toastr[type](text);
            else toastr[type](this.AlertMessages[text]);
        }
    }
}
