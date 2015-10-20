App.Collections.SearchToursResult = Backbone.Collection.extend({
    model: App.Models.ShortTourSearchResultItemModel,
});

App.Collections.MyToursCollection = Backbone.Collection.extend({
    model: App.Models.ShortTourItemModel,
});

App.Collections.MyBidsCollection = Backbone.Collection.extend({
    model: App.Models.BidItemModel,
});