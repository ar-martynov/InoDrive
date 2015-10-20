App.Models.TourModel = App.Models.BaseTourModel.extend({
    defaults: {
        expirationDate: '',
        freeSlots: '',
        startCity: '',
        destCity: '',
        payment: '',
        tempWayPoints: null,
        wayPoints: null,
        tourProfile: null,
    },
    url: String.format("{0}/tours/create", App.apiPath),

    initialize: function () {
        this.blacklist = ['startCity', 'destCity', 'tempWayPoints'];
        this.set('startCity', new App.Models.CityModel());
        this.set('destCity', new App.Models.CityModel());
        this.set('tourProfile', new App.Models.TourProfileModel());
        this.set('tempWayPoints', new App.Collections.WayPoints());
        this.searchResult = null;
    },

    parse: function (response) {
        if (!response.success) {
            this.url = String.format("{0}/tours/modify", App.apiPath);
            this.set('id', response.id);
            this.set('startCity', new App.Models.CityModel(response.wayPoints.shift()));
            this.set('destCity', new App.Models.CityModel(response.wayPoints.pop()));
            this.set('tourProfile', new App.Models.TourProfileModel(response.tourProfile));
            this.set('expirationDate', response.expirationDate);
            this.set('freeSlots', response.freeSlots);
            this.set('payment', response.payment);
            var self = this;
            _.each(response.wayPoints, function (tour) {
                self.get('tempWayPoints').add(new App.Models.CityModel(tour));
            });
        };
    },

    parseStartCity: function (latitude, longtitude, name, region) {

        this.start().setCity(latitude, longtitude, name, region);
        this.start().trigger('startCityComplete');

        if (App.debug) console.log('Start city added:', this.start().toJSON());
    },

    parseDestCity: function (latitude, longtitude, name, region) {

        this.dest().setCity(latitude, longtitude, name, region);
        this.dest().trigger('destCityComplete');

        if (App.debug) console.log('Destination city added: ', this.dest().toJSON());
    },

    wayPoints: function () {
        return this.get('tempWayPoints');
    },
    isWayPointExist: function (city) {
        var result = this.wayPoints().some(function (wayPointCity) {
            return city.getTitle() == wayPointCity.getTitle();
        });
        return result;
    },
    isWpntExistByLtLng: function (lt, lng) {
        var result = this.wayPoints().some(function (wayPointCity) {
            return (lt == wayPointCity.getLat() && lng == wayPointCity.getLong());
        });
        return result;
    },
    getWayPoint: function (city) {
        this.wayPoints().findWhere({ cityNameRu: event.item });
    },
    getProfile: function () {
        return this.get('tourProfile');
    }
});