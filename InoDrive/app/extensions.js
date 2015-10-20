//toastr options 
toastr.options.newestOnTop = false;
toastr.options.showEasing = 'swing';
toastr.options.hideEasing = 'linear';
toastr.options.positionClass = 'toast-bottom-right';

//pace options
paceOptions = {
    ajax: true,
    document: false,
    eventLag: true,
    elements: {
        selectors: ['#page']
    },
    restartOnPushState: true,
    //restartOnRequestAfter: false
};


// Extend the callbacks to work with Bootstrap, as used in this example
// See: http://thedersen.com/projects/backbone-validation/#configuration/callbacks
_.extend(Backbone.Validation.callbacks, {
    valid: function (view, attr, selector) {
        var $el = view.$('[name=' + attr + ']'),
            $group = $el.closest('.form-group');

        $group.removeClass('has-error');
        $group.find('.help-block').html('').addClass('hidden');
    },
    invalid: function (view, attr, error, selector) {
        var $el = view.$('[name=' + attr + ']'),
            $group = $el.closest('.form-group');

        $group.addClass('has-error');
        $group.find('.help-block').html(error).removeClass('hidden');
    }
});

/*
* Store a version of Backbone.sync to call from the
* modified version we create
*/
var backboneSync = Backbone.sync;
Backbone.sync = function (method, model, options) {
    /*
     * The jQuery `ajax` method includes a 'headers' option
     * which lets you set any headers you like
     */
    if (options == null) var options = {};
    //options.headers = {
    //    /* 
    //     * Set the 'Authorization' header and get the access
    //     * token from the `auth` module
    //     */
    //    'Authorization': App.session.get('token_type') + ' ' + App.session.get('access_token')
    //};
    if (!options.customError == true) options.error = function (response) {
        App.Alerts.showAlert(JSON.parse(response.responseText).message, "error", true);
    };

    /*
     * Call the stored original Backbone.sync method with
     * extra headers argument added
     */
    backboneSync(method, model, options);
};


//improving Backbone.remove() with recursion
Backbone.View.prototype.close = function () {
    if (App.debug) console.log(' DISPOSING | view: ', this);

    //remove all child views
    if (this.childViews != 'undefined' && this.childViews != null && this.childViews.length > 0) {
        this.closeChilds();
    }

    //Backbone.Validation .bind() unbinder
    if (this.stopValidation) this.stopValidation();
    //Backbone .listenTo() unbinder
    this.stopListening();
    //Backbone.Epoxy bindings unbinder
    this.removeBindings();
    //Backbone .on()/.bind() unbinder
    this.unbind();
    //jQuery .bind() unbinder
    $(this.el).unbind();

    // remove all models bindings
    // made by this view
    this.model.unbind(null, null, this);

    $(this.el).empty();
};

//Page Transitions 
//http://mikefowler.me/2013/11/18/page-transitions-in-backbone/
Backbone.View.prototype.goTo = function (view) {

    var previous = this.currentPage || null;
    var next = view;

    if (previous) {
        previous.transitionOut(function () {
            previous.remove();
        });
    }

    next.render({ page: true });
    this.$el.append(next.$el);
    next.transitionIn();
    this.currentPage = next;

},

Backbone.Epoxy.View.prototype.closeChilds = function () {
    var i;
    for (i = 0; i < this.childViews.length; ++i) {
        if (this.childViews[i] != undefined) this.childViews[i].close();
    }
};
///
function getExtension(fileName) {
    var parts = fileName.split('.');
    return parts[parts.length - 1];
};

function isImage(fileName) {
    var ext = getExtension(fileName);
    switch (ext.toLowerCase()) {
        case 'jpg':
        case 'gif':
        case 'bmp':
        case 'png':
            //etc
            return true;
    }
    return false;
};

String.format = function () {
    // The string containing the format items (e.g. "{0}")
    // will and always has to be the first argument.
    var theString = arguments[0];

    // start with the second argument (i = 1)
    for (var i = 1; i < arguments.length; i++) {
        // "gm" = RegEx options for Global search (more than one instance)
        // and for Multiline search
        var regEx = new RegExp("\\{" + (i - 1) + "\\}", "gm");
        theString = theString.replace(regEx, arguments[i]);
    }

    return theString;
}
