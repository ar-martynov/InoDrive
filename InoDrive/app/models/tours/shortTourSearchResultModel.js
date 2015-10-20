App.Models.ShortTourSearchResultItemModel = Backbone.Epoxy.Model.extend({
    defaults: {
        id: 0,
        owner: '',
        avatarPath: '',
        expirationDate: '',
        startCity: '',
        destCity: '',
        freeSlots: '',
        payment: '',
    },
    computeds: {
        getTourUrl: function () {
            return '#/tours/search/tour/' + this.get('id');
        }
    }
});

App.Models.ShortTourItemModel = Backbone.Epoxy.Model.extend({
    defaults: {
        id: 0,
        owner: '',
        expirationDate: '',
        isExpired: '',
        startCity: '',
        destCity: '',
        freeSlots: '',
        payment: '',
        
    },
    urlRoot: App.apiPath + '/tours',
    isExpired: function () {
        return this.get('isExpired');
    },
    computeds: {
        getTourUrl: function () {
            return String.format('#/tours/tour/{0}', this.get('id'));
        },
        getModifyTourUrl: function () {
            return String.format('#/tours/edit/{0}', this.get('id'));
        },
        getRemoveTourUrl: function () {
            return String.format('#/tours/delete/{0}', this.get('id'));
        }
    }
});