App.Router = Backbone.Router.extend({
    routes: {
        '': 'index',
        'index': 'index',
        'tours': 'getTours',
        'tours/edit/:id': 'editTour',
        'tours/tour/:id': 'getTour',
        'tours/create': 'createTour',
        'tours/search': 'searchTour',
        'tours/search/tour/:id': 'getTour',
        'bids': 'bids',
        'bids/mybids' : 'myBids',
        'user/profile': 'userProfile',
        '*path': 'notFound',
    },

    initialize: function () {
        _.bindAll.apply(_, [this].concat(_.functions(this)));

        //global models 
        App.Models.search = new App.Models.TourSearchModel();
        
        //globals
        App.Views.header = new App.Views.HeaderView({ model: App.session.user });
    },

    checkSession: function (alert) {
        if (App.session.status()) return true;

        if(!alert)App.Alerts.showAlert("NotAuthorized", "error");
        return false;
    },

    index: function () {
        App.Views.Dispose();
        App.Views.CurrentView = (new App.Views.SearchTourView({ model: App.Models.search, newSearch: true })).render(); 

    },

    searchTour: function () {
        App.Views.Dispose();
        App.Views.CurrentView = (new App.Views.SearchTourView({ model: App.Models.search, newSearch: false })).render();
    },

    getTour: function (id) {
        var model = new App.Models.TourSearchResultModel({ id: id });
        model.fetch({
            success: function (response) {
                App.Views.Dispose();
                App.Views.CurrentView = (new App.Views.TourView({ model: response })).render();
            }
        });
    },

    getTours: function () {
        if (this.checkSession()) {
            var model = new App.Models.MyToursModel();
            model.fetch({
                success: function (response) {
                    App.Views.Dispose();
                    App.Views.CurrentView = (new App.Views.UserToursView({ model: model })).render();
                }
            });
        } else this.navigate('/', { trigger: true });
    },

    createTour: function () {
        var model = new App.Models.TourModel();
        if (this.checkSession(true)) {
            model.getProfile().fetch({
                url: String.format("{0}/tours/lastProfile", App.apiPath),
                success: function () {
                    App.Views.Dispose();
                    App.Views.CurrentView = (new App.Views.CreateTourView({ model: model, el: '#page' })).render();
                }
            });
        } else {
            App.Views.Dispose();
            App.Views.CurrentView = (new App.Views.CreateTourView({ model: model, el: '#page' })).render();
        }
    },

    editTour: function (id) {
        if (this.checkSession()) {
            var model = new App.Models.TourModel();
            model.fetch({
                url: String.format("{0}/tours/modify?id={1}", App.apiPath, id),
                success: function (response) {
                    App.Views.Dispose();
                    App.Views.CurrentView = new App.Views.CreateTourView({ model: response });
                    App.Views.CurrentView.template = App.Templates.get('tours/tourModify');
                    App.Views.CurrentView.render();
                },
            });
        } else this.navigate('/', { trigger: true });
    },

    bids: function () {
        if (this.checkSession()) {
            var model = new App.Models.BidsToursModel();
            model.fetch({
                success: function (response) {
                    App.Views.Dispose();
                    App.session.user.clearAdsCount();
                    App.Views.CurrentView = (new App.Views.BidsView({ model: model })).render();
                }
            });
        } else this.navigate('/', { trigger: true });
    },

    myBids: function () {
        if (this.checkSession()) {
            var model = new App.Models.BidsToursModel();
            model.fetch({
                url: String.format("{0}/bids/MyBids?page=1&showCompleted=true", App.apiPath),
                success: function (response) {
                    App.Views.Dispose();
                    App.session.user.clearBidsCount();
                    App.Views.CurrentView = (new App.Views.UserBidsView({ model: model })).render();
                }
            });
        } else this.navigate('/', { trigger: true });
    },  

    userProfile: function () {
        if (this.checkSession()) {
            App.Views.Dispose();
            App.Views.CurrentView = (new App.Views.MyProfileView({ model: App.session.user })).render();
        } else this.navigate('/', { trigger: true });
    },

    

    notFound: function () {
       // App.router.navigate('/', { trigger: true });
        App.Alerts.showAlert("PathNotFound", "error");
    },

});