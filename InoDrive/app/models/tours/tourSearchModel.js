App.Models.TourSearchModel = App.Models.TourModel.extend({
    defaults: {
        expirationDate: '',
        freeSlots: '',
        startCity: '',
        destCity: '',
        advancedSearchProfile: null,
        wayPoints: null,
        totalPages: 1,
        page: 1,
    },
    url: String.format("{0}/tours/search", App.apiPath),
    initialize: function () {
        this.searchUrlContainer = {};
        this.searchUrlContainer[false] = this.url;
        this.searchUrlContainer[true] = App.apiPath + "/tours/advancedSearch";

        this.set('startCity', new App.Models.CityModel());
        this.set('destCity', new App.Models.CityModel());
        this.set('advancedSearchProfile', new App.Models.TourAdvancedSearchModel());
        this.set('wayPoints', new App.Collections.WayPoints());

    },
    resetPaging: function () {
        this.set('page', 1);
        this.set('totalPages', 1);
    },
    swapUrl: function (flag) {
        if (flag) this.blacklist = ['tours'];
        else this.blacklist = ['wayPoints', 'advancedSearchProfile', 'tours'];
        this.url = this.searchUrlContainer[flag];
    },
    wayPoints: function () {
        return this.get('wayPoints');
    },
    getCurrentPage: function () {
        return this.get('page');
    },
    getTotalPages: function () {
        return this.get('totalPages');
    },
    getAdvancedSearchProfile: function () {
        return this.get('advancedSearchProfile');
    }
});
