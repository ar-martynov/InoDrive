App.Templates = (function () {
    var url = 'app/templates/',
        isCached = true,
        cache = [],
        getTemplate = function (name) {
            if (cache[name] != null && isCached) {
                return cache[name];
            }
            else {
                var strUrl = url + name + ".html";
                var strReturn = "";
                $.ajax({
                    url: strUrl,
                    async: false,
                    //cache: isCached,
                    global: false,
                    success: function (html) {
                        strReturn = html;
                    }
                });
                if (strReturn != '') {
                    cache[name] = strReturn;
                }
                return strReturn;
            }
        }
    return {
        //return HTML template
        get: function (templateName) {
            return getTemplate(templateName);
        },
        //return HTML template and compiles JavaScript templates into functions that can be evaluated for rendering.
        getUnderscore: function (templateName, data) {
            return _.template(getTemplate(templateName), data);
        }
    };
})();