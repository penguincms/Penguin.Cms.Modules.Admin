$(document).ready(function () {
	$('button').button();

	$('.ToggleLeftPane').click(function () {
		if ($('#LeftPane').hasClass('visible')) {
			$('#LeftPane').removeClass('visible');
			$('#LeftPane').hide("slide", { direction: "left" }, "slow");
		} else {
			$('#LeftPane').addClass('visible');
			$('#LeftPane').show("slide", { direction: "left" }, "slow");
		}
	});
});

function elementHasProperty(element, name) {
    var attr = $(element).attr(name);

    if (typeof attr !== typeof undefined && attr !== false) {
        return true;
    } else {
        return false;
    }
}

function updateQueryStringParameter(key, value, uri) {
    var LiveUrl = false;
    if (!uri) {
        LiveUrl = true;
        uri = window.location.href;
    }

    var newUri;

    var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
    var separator = uri.indexOf('?') !== -1 ? "&" : "?";
    if (uri.match(re)) {
        newUri = uri.replace(re, '$1' + key + "=" + value + '$2');
    }
    else {
        newUri = uri + separator + key + "=" + value;
    }

    if (!LiveUrl) {
        return newUri;
    } else {
        window.location = newUri;
        return false;
    }
}

function SubmitForm(scallback, ecallback) {
    e.preventDefault();
    var url = $(this).closest('form').attr('action');
    var data = $(this).closest('form').serialize();

    $.ajax({
        url: url,
        type: 'post',
        data: data,
        success: scallback,
        error: ecallback || scallback
    });
}

/* Jquery UI crap */

$(document).ready(function () {
    $("a").attr("rel", "external");
    $("form").attr("data-ajax", "false");
});

var Site = {
    Loaders: 0,
    ShowLoader: function () {
        if (this.Loaders === 0) {
            $('body').append('<div id="loaderModel"><div id="loader"></div></div>');
        }

        this.Loaders++;
    },
    HideLoader: function () {
        this.Loaders--;

        if (this.Loaders === 0) {
            $('#loaderModel').remove();
        }
    },
    PostJsonAsForm: function (jsonObject, Url, CallBack) {

        Site.ShowLoader();
        if (CallBack) {
            if (!Url) {
                Url = window.location.href;
            }

            var form_data = new FormData();

            for (var key in jsonObject) {
                form_data.append(key, jsonObject[key]);
            }

            $.ajax({
                url: Url,
                data: form_data,
                processData: false,
                contentType: false,
                type: 'POST'
            }).done(function (data) {
                Site.HideLoader();
                CallBack(data);
            });
        } else {
            var form = $('<form></form>');
            form.attr('id', 'jsonForm');
            form.attr('method', 'POST');
            form.attr('action', Url);
            form.css('display', 'none');

            for (var k in jsonObject) {
                var input = $('<input />');

                input.val(jsonObject[k]);
                input.attr('name', k);

                form.append(input);
            }

            $('body').append(form);

            $('#jsonForm').submit();

        }
    }
};

var Functions = {
    XmlToJson: function (xml) {

        // Create the return object
        var obj = {};

        if (xml.nodeType === 1) { // element
            // do attributes
            if (xml.attributes.length > 0) {
                obj["@attributes"] = {};
                for (var j = 0; j < xml.attributes.length; j++) {
                    var attribute = xml.attributes.item(j);
                    obj["@attributes"][attribute.nodeName] = attribute.nodeValue;
                }
            }
        } else if (xml.nodeType === 3) { // text
            obj = xml.nodeValue;
        }

        // do children
        // If just one text node inside
        if (xml.hasChildNodes() && xml.childNodes.length === 1 && xml.childNodes[0].nodeType === 3) {
            obj = xml.childNodes[0].nodeValue;
        }
        else if (xml.hasChildNodes()) {
            for (var i = 0; i < xml.childNodes.length; i++) {
                var item = xml.childNodes.item(i);
                var nodeName = item.nodeName;
                if (typeof obj[nodeName] === "undefined") {
                    obj[nodeName] = Functions.XmlToJson(item);
                } else {
                    if (typeof obj[nodeName].push === "undefined") {
                        var old = obj[nodeName];
                        obj[nodeName] = [];
                        obj[nodeName].push(old);
                    }
                    obj[nodeName].push(Functions.XmlToJson(item));
                }
            }
        }
        return obj;
    }
};