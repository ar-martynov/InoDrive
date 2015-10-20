App.Models.CityModel = Backbone.Epoxy.Model.extend({
    defaults: {
        cityNameRu: '',
        regionNameRu: '',
        latitude: '',
        longtitude: '',
    },
    setCity: function (lat, long, city, region) {
        this.set('latitude', lat);
        this.set('longtitude', long);
        this.set('cityNameRu', city);
        this.set('regionNameRu', region);
    },
    isCityExist: function (city) {
        if (city.getLat() == this.getLat() && city.getLong() == this.getLong() || city.getTitle() == this.getTitle()) return true;
        return false;
    },
    getLat: function () {
        return this.get('latitude');
    },
    getLong: function () {
        return this.get('longtitude');
    },
    getName: function () {
        return this.get('cityNameRu');
    },
    getTitle: function () {
        return this.get('regionNameRu') + ', ' + this.get('cityNameRu');
    },
    isEmpty: function () {
        if (this.get('cityNameRu') == '' && this.get('regionNameRu') == '' || this.get('latitude') == '' && this.get('longtitude') == '') return true;
        return false;
    }
});


