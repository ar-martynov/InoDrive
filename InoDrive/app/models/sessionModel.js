App.Models.Session = Backbone.Model.extend({
    // Initialize with negative/empty defaults
    // These will be overriden after the initial checkAuth

    defaults: {
        logged_in: false,
        access_token: '',
        token_type: '',
        expires_in: '',
        refresh_token: '',
        user_id: '',
        signalR_userId:'',
    },

    initialize: function () {
        _.bindAll.apply(_, [this].concat(_.functions(this)));
        // Singleton user object
        // Access or listen on this throughout any module with App.session.User
        this.user = new App.Models.User();
        this.rememberMeFlag = false;
        this.checkSession();
    },

    cookies: {
        getCookie: function (name) {
            var matches = document.cookie.match(new RegExp(
              "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
            ));
            return matches ? decodeURIComponent(matches[1]) : undefined;
        },

        setCookie: function (name, value, options) {
            options = options || {};

            var expires = options.expires;

            if (typeof expires == "number" && expires) {
                var d = new Date();
                d.setTime(d.getTime() + expires * 1000);
                expires = options.expires = d;
            }
            if (expires && expires.toUTCString) {
                options.expires = expires.toUTCString();
            }

            value = encodeURIComponent(value);

            var updatedCookie = name + "=" + value;

            for (var propName in options) {
                updatedCookie += "; " + propName;
                var propValue = options[propName];
                if (propValue !== true) {
                    updatedCookie += "=" + propValue;
                }
            }

            document.cookie = updatedCookie;
        },

        deleteCookie: function (name) {
            this.setCookie(name, "", {
                expires: -1
            })
        }
    },

    setAuthRefreshTimeOut: function (timeout) {
        if (timeout > 240) timeout = (timeout - 90)*1000;
        var self= this;
        this.sessionRefresher = setInterval(function () { self.checkAuth() }, timeout);
    },

    url: function () {
        return String.format("{0}/token", App.apiPath);
    },

    getAuthUrl: function(){
        return String.format("refresh_token={0}&grant_type=refresh_token&client_id={1}&client_secret={2}",
                             this.get('refresh_token'),
                             App.apiClientId,
                             App.apiClientSecret);
    },

    status: function () {
        
        return this.get('logged_in');
    },

    startSignalRConnection: function () {
        if (!App.Hubs.bidsHub) App.Hubs.bidsHub = new App.Hubs.BidsHubWrapper();
        App.Hubs.bidsHub.start(this.get('access_token'), this.get('user_id'));
    },

    stopSignalRConnections: function () {
        App.Hubs.bidsHub.stop();
    },

    //add refresh token to storage
    keyToStorage: function (token, type) {
        localStorage.setItem('userToken', token);
        localStorage.setItem('tokenType', type);
        this.cookies.setCookie('token', this.get('access_token'), { expires: this.get('expires_in') });
       
    },

    //get refresh token from storage
    keyFromStorage: function () {
        var token = localStorage.getItem('userToken');
        var type = localStorage.getItem('tokenType');
        if (token && type) {
            if (this.cookies.getCookie('token') != null) {
                this.cookies.deleteCookie('token');
            }
            this.set('refresh_token', token);
            this.set('token_type', type);
            return true;
        }
        else return false;
    },

    clearStorage: function(){
        localStorage.removeItem("userToken");
        localStorage.removeItem("tokenType");
        this.cookies.deleteCookie('token');
    },

    //check previous user session if he was logged and pass 'rememberMe' flag
    checkSession: function () {
        if (this.keyFromStorage() == true) {
            var self = this;
            $.ajax({
                url: this.url(),
                type: "POST",
                contentType: 'application/json',
                dataType: 'json',
                data: self.getAuthUrl(),
                success: function (response) {
                    self.updateSession(response);

                    self.keyToStorage(response.refresh_token, response.token_type);

                    self.setAuthRefreshTimeOut(response.expires_in);

                    self.startSignalRConnection();

                    self.user.fetch(
                   {
                       success: function () {
                           self.set('logged_in', true);
                           self.trigger('statusChanged');
                           if (App.debug) console.log("USER FETCHED::", App.session.user.toJSON());
                       },
                       error: function () {
                           if (App.debug) console.log('Failed to fetch user model!');
                       },
                       complete: function () {
                           if (App.debug) console.log('Failed to fetch user model!');
                       },
                   });
                    if (App.debug) console.log("SESSION REFRESHED ::", App.session.toJSON());
                }
            });
        }
    },

    // Fxn to update user attributes after recieving API response
    updateSession: function (response) {
        this.set('refresh_token', response.refresh_token);
        this.set('access_token', response.access_token);
        this.set('token_type', response.token_type);
        this.set('expires_in', response.expires_in);
        this.set('user_id', response.user_id);
    },

    /*
    * Check for session from API
    * The API will parse client_token using its secret_token
    * and return a user object if authenticated
    */
    checkAuth: function () {
        var self = this;
        $.ajax({
            url: this.url(),
            type: "POST",
            contentType: 'application/json',
            dataType: 'json',
            data: self.getAuthUrl(),
            success: function (response) {
                self.updateSession(response);

                if (self.rememberMeFlag) self.keyToStorage(response.refresh_token, response.token_type);
                else self.cookies.setCookie('token', response.access_token, { expires: response.expires_in });

                if (App.debug) console.log("SESSION REFRESHED ::", self.toJSON());
            },
            error: function () {
                self.logout();
                App.Alerts.showAlert('SessionRefreshError', 'error');
            }
        });
    },

    /*
    * Abstracted fxn to make a POST request to the auth endpoint
    * This takes care of the token for security, as well as
    * updating the user and session after receiving an API response
    */
    postAuth: function (authModel) {
        var self = this;
        $.ajax({
            url: this.url(),
            type: "POST",
            contentType: 'application/json',
            dataType: 'json',
            data: authModel.getParamString(authModel.attributes),

            success: function (response) {
                self.updateSession(response);
                Pace.restart();
                if (authModel.get('rememberMe')) {
                    self.keyToStorage(response.refresh_token, response.token_type);
                    self.rememberMeFlag = true;
                }
                else self.cookies.setCookie('token', response.access_token, { expires: response.expires_in });

                self.setAuthRefreshTimeOut(response.expires_in);

                self.startSignalRConnection();
               

                self.user.fetch(
                    {
                        success: function () {
                            self.set('logged_in', true);
                            self.trigger('statusChanged');
                        },
                        error: function () {
                            if (App.debug) console.log('Failed to fetch!');
                        }
                    });

                App.Views.CurrentModalView.teardown();
                if (App.debug) console.log("LOGIN OK::", App.session.user.toJSON(), App.session.toJSON());
            },
            error: function (response) {
                var errors = JSON.parse(response.responseText)['error_description'];
                Backbone.Validation.callbacks.invalid(App.Views.CurrentModalView, 'userName', errors);
                Backbone.Validation.callbacks.invalid(App.Views.CurrentModalView, 'password', '');
                authModel.trigger('validated:invalid', App.Views.CurrentModalView, 'userName', errors);
                authModel.trigger('validated:invalid', App.Views.CurrentModalView, 'password', '');
            }
        });
    },

    logout: function () {
        this.clear().set(this.defaults);
        clearInterval(this.sessionRefresher);
        this.clearStorage();
        this.stopSignalRConnections();
        this.trigger('statusChanged');
        Pace.restart();
        App.router.navigate('/', { trigger: true });
    },
});