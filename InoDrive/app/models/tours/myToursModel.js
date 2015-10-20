App.Models.MyToursModel = Backbone.Epoxy.Model.extend({
    defautlts: {
        tours: null,
        page: 1,
        totalPages: 1,
    },
    url: String.format("{0}/tours/myTours?page=1", App.apiPath),
    initialize: function () {
        this.set("tours", new App.Collections.MyToursCollection());
    },
    setPage: function (value) {
        this.set('page', value);
        this.url = String.format("{0}/tours/myTours?page={1}", App.apiPath, value);
    },
    getTours: function () {
        return this.get('tours');
    },
    getCurrentPage: function () {
        return this.get('page');
    },
    getTotalPages: function () {
        return this.get('totalPages');
    },
    parse: function (response) {
        if (App.debug) console.log("User tours: ", response);
        var self = this;
       
        this.getTours().reset();
       
        _.each(response.tours, function (tour) {
            self.getTours().add(new App.Models.ShortTourItemModel(tour));
        });
        this.set('totalPages', response.totalPages);
    },
});