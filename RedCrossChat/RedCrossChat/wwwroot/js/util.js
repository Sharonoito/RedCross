var isLoading = true;
var previewModal = [];
var requestData = null;

$.ajaxSetup({
    cache: false
});

$(document).ajaxSend(function (e, response, options) {
    var preventLoading = false;
    if (!isLoading) {
        preventLoading = true;
        isLoading = true;
    } else {
        if (options.url) {
            if (options.url.toString().indexOf("sidx") !== -1) {
                preventLoading = true;
            }
        }
        //if (options.port) {

        //} else if (options.url) {
        //    if (options.url.toString().indexOf("sidx") !== -1) {
        //        preventLoading = true;
        //    }
        //}
    }

    if (!preventLoading) {
        $("#loading-panel").show();
        $("#loading-panel").animate({ "right": "0px" }, "slow");
    }
});

$(document).ajaxComplete(function (event, xhr, options) {
    $("#loading-panel").animate({ "right": "-300px" }, "slow", function () {
        $("#loading-panel").hide();
    });

    if (xhr && xhr.Data && xhr.Data.LogOnUrl) {
        if (xhr.status === 401) {
            window.location.href = xhr.Data.LogOnUrl;
            return;
        }
    }
    preparedDate();
    preparedMultiDate();
    preparedHtmlEditor();
    //preparedSummernoteEditor();
    //console.log("preparedSummernoteEditor1111");
    prepareChosenSelect(false);
    prepareSelect2();
    prepareCheckboxes(false);
    //console.log("Ajax Function!!!");
});

$(document).ajaxError(function (event, xhr, options, exception) {
    if (xhr.status === 0) {
        xhr.abort();
    }
    if (xhr.status === 401) {
        if (xhr.Data && xhr.Data.LogOnUrl)
            window.location.href = xhr.Data.LogOnUrl;
        return;
    }
});

$(document).ajaxSuccess(function (event, xhr, settings) {
    if (xhr && xhr.responseJSON && xhr.responseJSON.LogOnUrl) {
        window.location.href = xhr.responseJSON.LogOnUrl;
        return;

    }
});

$(function () {
    validateNumber();
    preparedDate();
    preparedMultiDate();
    preparedHtmlEditor();
    //preparedSummernoteEditor();
    //console.log("preparedSummernoteEditor1111");
    prepareSelect2();
    prepareChosenSelect(false);
    prepareCheckboxes(false);
    //console.log("Main Function!!!");
});

$(document).on('click', '#changePass', function () {
    $('li.open').removeClass('open');
});

$(document).on("dp.change", ".date-from", function (e) {
    var to = $(this).data('date-to');
    $('#' + to).data("DateTimePicker").setMinDate(e.date);
});

$(document).on("dp.change", ".date-to", function (e) {
    var from = $(this).data('date-from');
    $('#' + from).data("DateTimePicker").setMaxDate(e.date);
});

$(document).on("shown.bs.tab", function (e) {
    var href = $(e.target).attr("href");
    if (href) {
        var id = href.substring(href.indexOf("#"), href.length);
        //resizeAllGrids($(".tab-pane" + id));
    }
});

$(document).on("click", ".add-filter", function () {
    var option = $(this).next().find('select option:selected');
    var value = $(option).val();
    if (value !== '-1') {
        var model = {
            type: $(option).data('type'),
            description: $(option).text().trim(),
            name: value,
            url: $(option).data('url')
        };
        //$('.btn-filter').removeClass('hide');
        var innerForm = $(this).data("formid");
        var selector = '#' + innerForm + ' div:first';
        var html = createFilter(model);
        $(selector).prepend(html);
        preparedDate();
        preparedMultiDate();
        $(option).hide();
        $(this).next().find('select').val('-1');
    }
});

$(document).on('click', '.close-filter', function () {
    var value = $(this).data("value");
    var div = $(this).closest('div');
    var parentDiv = div.parent();

    $('.select-filter option[value=' + value + ']').show();
    resetFilter($(this).closest('form'));
    $(div).remove();

    if ($(parentDiv).find('.control-filter').length === 0) {
        $(parentDiv).find('.btn-filter').addClass('hide');
    }
});

$(document).on('click', '.manually-change>i.fa-toggle-off', function () {
    var input = $(this).closest("label").find("input:text");

    $(input).attr("readonly", "readonly");
    $(input).data("manually-changed", false);
    $(this).removeClass("fa-toggle-off").addClass("fa-toggle-on");
});

$(document).on('click', '.manually-change>i.fa-toggle-on', function () {
    var input = $(this).closest("label").find("input:text");

    $(input).removeAttr("readonly");
    $(input).data("manually-changed", true);
    $(this).removeClass("fa-toggle-on").addClass("fa-toggle-off");
});

$(document).on('click', 'a.btn-disabled button.btn-disabled', function (event) {
    if (event) {
        event.preventDefault();
        event.stopPropagation();
    }
    return false;
});

$(document).on('click', '.addImg', function () {
    $('#uploadImg').trigger('click');
    $(this).addClass('pendding');
});

$('.modal').scroll(function () {
    $(".bootstrap-datetimepicker-widget").hide();
});

$(document).on("hide.bs.modal", ".modal", function () {
    //console.log("called => hide.bs.modal, .modal");
    clearValidationResult();
    var cls = $(this).data("clearonclose");
    var callBack = $(this).data("callback");
    if (callBack) {
        //console.log("called => hide.bs.modal, .modal", callBack);
        callFn(callBack, $(this).data("invoker"));
    }

    if (cls === undefined) {
        cls = true;
    }

    if (cls) {
        clearFormElem(this);
    }
    var clearHtml = $(this).data('emptyonclose');
    if (clearHtml) {
        $(this).find(".modal-content").html("");
    }
    previewModal.pop();
    var modal = previewModal.pop();
    if (modal !== this) {
        $(modal).addClass('in');
        //$(this).data("clearonclose", true);
    }
});

$(document).on('hidden.bs.modal', '.modal', function () {
    //console.log("called => hidden.bs.modal, .modal");
    $('.modal:visible').length && $(document.body).addClass('modal-open');
});

$(document).on("shown.bs.modal", ".modal", function () {
    //console.log("called => shown.bs.modal, .modal");
    enableBtn(this);
    preparedDate();
    preparedMultiDate();
    //resizeAllGrids(this);

    var init = $(this).data("init");

    if (init) {
        callFn(init, $(this).data("invoker"));
    }
    var modal = $('.modal').not(this).filter(function () {
        return $(this).hasClass('in');
    }).removeClass("in").data("clearonclose", false);
    previewModal.push(modal);
    var index = previewModal.indexOf(this);
    if (index === -1) {
        previewModal.push(this);

    }
});

$(document).on("change", ".reloadChild", function () {
    var child = $(this).data('childid');
    var url = $(this).data('url');
    var value = $(this).val();
    loadSelectUrl(child, url, value);
});

$(document).on("click", "a.delete:not(.disabled),li.delete:not(.disabled),button.delete:not(.disabled)", function (event, preventConfirm) {
    var $element = $(this),
        dTables = $element.data("table") ? $element.data("table").split(' ') : undefined,
        url = $element.data('action'),
        id = $element.data('id'),
        callback = $element.data('callback');
    //console.log("callback!", callback);
    //console.log("url", url);
    //console.log("$element", $element.data('action'));

    var cont = function (data) {
        //console.log("data", data);
        if (dTables !== undefined) {
            if (dTables && dTables !== '') {
                for (var i = 0; i < dTables.length; i++) {
                    $('#' + dTables[i]).DataTable().ajax.reload();
                }
            }
        }

        callFn(callback, data, $element);
    };

    confirm(event, "Are you sure you want to delete this record?", function () {
        //console.log("url", url);
        if (url) {
            $.ajax({
                type: "get",
                url: url,
                data: {
                    'id': id
                },
                success: function (response) {
                    if (response.success) {
                        success(response.message);
                        if (response.requestData) {
                            requestData = response.requestData;
                        }

                        cont(response);
                    } else if (response.error === "NotAuthorized") {
                        error("You are not authorized to complete this action!");
                    } else {
                        error(response.message || "Oops, an error occured, please try again!");
                    }
                },
                error: function (err) {
                    error(err || "Oops, an error occured, please try again!");
                },
                dataType: 'json'
            });
        } else {
            cont(id);
        }
    });
});

$(document).on("click", "a.toggle-status:not(.disabled),li.toggle-status:not(.disabled),button.toggle-status:not(.disabled)", function (event, preventConfirm) {
    var $element = $(this),
        dTables = $element.data("table") ? $element.data("table").split(' ') : undefined,
        url = $element.data('action'),
        id = $element.data('id'),
        callback = $element.data('callback'),
        message = $element.data('message');

    var cont = function (data) {
        if (dTables !== undefined) {
            if (dTables && dTables !== '') {
                for (var i = 0; i < dTables.length; i++) {
                    $('#' + dTables[i]).DataTable().ajax.reload();
                }
            }
        }
        callFn(callback, data, $element);
    };

    //var newStatus = status === 1 ? 0 : 1;
    //var newState = newStatus === 1
    //    ? '<span class="badge badge-success">Active</span>'
    //    : '<span class="badge badge-warning">Inactive</span>';
    //var currState = newStatus === 1
    //    ? '<span class="badge badge-warning">Inactive</span>'
    //    : '<span class="badge badge-success">Active</span>';

    //var message =  "This will toggle the status from " + currState + " to " + newState + "!";
    confirm(event, message, function () {
        if (url) {
            $.ajax({
                type: "post",
                url: url,
                data: {
                    'id': id
                },
                success: function (response) {
                    if (response.success) {
                        success(response.message);
                        if (response.requestData) {
                            requestData = response.requestData;
                        }
                        cont(response);
                    } else if (response.error === "NotAuthorized") {
                        error("You are not authorized to complete this action!");
                    } else {
                        error(response.message || "Oops, an error occured, please try again!");
                    }
                },
                error: function (err) {
                    error(err || "Oops, an error occured, please try again!");
                },
                dataType: 'json'
            });
        } else {
            cont(id);
        }
    });
});

$(document).on("submit", "form.modal-Crud", function () {
    //console.log("called => submit, form.modal-Crud");
    try {
        var $form = $(this);

        disableBtn($form);
        var callInit = $form.data("callinit");
        if (callInit && callInit !== '') {
            callFn(callInit);
        }
        var data = getCrudFields($form),
            dTables = $form.data("table") ? $form.data("table").split(' ') : undefined,
            modal = $form.data("modal"),
            url = $form.attr('action'),
            callback = $form.data("callback");

        //console.log('modal: ', modal);
        //console.log('data: ', data);
        //console.log('callback: ', callback);
        //console.log('form: ', $form);

        var cont = function () {
            if (dTables !== undefined) {
                if (dTables && dTables !== '') {
                    for (var i = 0; i < dTables.length; i++) {
                        $('#' + dTables[i]).DataTable().ajax.reload();
                    }
                }
            }
            //console.log('data: ', data);

            if (callback && callback !== '') {
                callFn(callback, data, $("#" + modal).data("invoker"));
                //callFn(callback);
            }

            if (modal && modal !== '') {                
                $('#' + modal).modal('hide');
            }
        };

        if (url && url !== '') {
            //console.log('url', url);
            $.ajax({
                type: "POST",
                url: url,
                data: data,
                dataType: 'json',
                success: function (response) {
                    console.log('submit: ', response);
                    enableBtn($form);
                    if (response.success === true) { // OK
                        clearValidationResult();
                        success(response.message || "Saved successfully.");

                        if (response.responseData) {
                            // Created Id will have the id for the created entity
                            if (response.responseData.createdId) {
                                // create an object and add it to data
                                var createdId = { name: 'CreatedId', value: response.responseData.createdId } // TODO -> Assing in controller
                                data.push(createdId);
                            }
                            
                            //data.push(response.responseData);
                        }
                        //console.log('calling cont after saving');
                        cont();
                    } else if (response.valid === false) { // error de validacion                  
                        showValidationResult(response);
                    } else if (response.error === "NotAuthorized") { // not authorized
                        error("You are not authorized to complete this action!");
                    } else { // error del servidor
                        error(response.message.length === 0 ? "Oops, an error occured, please try again!" : response.message);
                    }
                },
                error: function (e) {
                    enableBtn($form);
                    error("Oops, an error occured, please try again!");
                },
            });
        } else {
            cont();
        }
    }
    catch (err) {
        console.log("Caught Err: ", err.message);
    }
    return false;
});

$(document).on("click", ".load-modal", function () {
    //console.log("called => click, form.load-modal");
    var beforeLoad = $(this).data("beforeload");
    if (beforeLoad) {
        callFn(beforeLoad, this);
    }

    var $element = $(this),
        modal = $element.data("modal") || $element.closest("ul").data("modal"),
        url = $element.data('action'),
        id = $element.data('id'),
        hiddenId = $element.data('hidden'),
        callback = $element.data('callback'),
        viewUrl = $element.data('viewurl'),
        content = $element.data('content') ? $element.data('content') : ".modal-content";

    $("#" + modal).data("invoker", $element);
    console.log('modal:', modal);

    if (hiddenId) {
        $("#" + hiddenId).val(id);
    }

    var data = { 'id': id };
    var dataType = "json";
    if (viewUrl) {
        data = "";
        dataType = "html";
    }

    console.log('dataType:', dataType);
    console.log('viewUrl:', viewUrl);
    console.log('url:', url);

    if (url) {
        $.ajax({
            type: "get",
            url: url,
            data: data,
            dataType: dataType,
            success: function (response) {
                //console.log('response1', response);
                //console.log('response.error1', response.error);
                if (response.error == "NotAuthorized") {
                    //console.log('response.error2', response.error);
                    //if (modal) { $('#' + modal).modal("hide"); }
                    //$('#' + modal).modal("hide");

                    error("You are not authorized to complete this action!");
                    setTimeout(function () {
                        $('#' + modal).modal("hide");
                    }, 2000);
                    // Reload same page - Sirme
                    //window.location.href = window.location.href;
                } else {
                    console.log('Tumepita');
                    if (viewUrl) {
                        try {
                            var obj = JSON.parse(response);
                            if (obj.success === false) {
                                //$('#' + modal).modal("hide");
                                setTimeout(function () {
                                    $('#' + modal).modal("hide");
                                }, 2000);

                                if (obj.error === "NotAuthorized") {
                                    //console.log('obj.error', obj.error);
                                    return error("You are not authorized to complete this action!");
                                } else {
                                    error(obj.message || "Error");
                                }
                                return;
                            }
                        } catch (e) {
                            // do nothing, it's html content
                        }

                        console.log("content",content);

                        console.log("response",response)
                        $('#' + modal).find(content).html(response);
                        // $('#' + modal).html(response);

                        if (hiddenId) {
                            $("#" + hiddenId).val(id);
                        }

                        jQuery.validator.unobtrusive.parse('#' + modal);
                        if (callback)
                            callFn(callback, id);

                    } else {
                        if (callback)
                            callFn(callback, response);
                    }

                    if (modal) {
                        //console.log('Showing');
                        $('#' + modal).modal();
                    }
                }
            },
            error: function (e) {
                error("Error");
            }
        });
    } else {
        //console.log('No Url');
        if (modal) {
            //console.log('Showing');
            $('#' + modal).modal('show');
        }
        //console.log('We got here');
        callFn(callback, id);
    }

    return false;
});

$(document).on("click", "button.submit", function () {
    //console.log("click button.submit");
    var $element = $(this),
        dTables = $element.data("table") ? $element.data("table").split(' ') : undefined,
        url = $element.data('action'),
        id = $element.data('id'),
        callback = $element.data('callback');
    //console.log("callback!", callback);

    var cont = function (data) {
        //console.log("data!", data);
        if (dTables !== undefined) {
            if (dTables && dTables !== '') {
                for (var i = 0; i < dTables.length; i++) {
                    $('#' + dTables[i]).DataTable().ajax.reload();
                }
            }
        }

        callFn(callback, data, $element);
    };

    if (url) {
        $.ajax({
            type: "get",
            url: url,
            data: {
                'id': id
            },
            success: function (response) {
                //console.log('response', response);
                if (response.success) {
                    success(response.message);
                    if (response.requestData) {
                        requestData = response.requestData;
                    }

                    //cont(response);
                } else if (response.error === "NotAuthorized") {
                    error("You are not authorized to complete this action!");
                } else {
                    error(response.message || "Oops, an error occured, please try again!");
                }
            },
            error: function (err) {
                //console.log('err', err);
                error(err || "Oops, an error occured, please try again!");
            },
            dataType: 'json'
        });
    } else {
        cont(id);
    }
});

$(document).on("keypress", "input:text", function (e) {
    //SAM Removed this as it was disabling typing of leading zeros
    //if ($(this).val() === "0" && e.keyCode !== 46 && e.charCode !== 46) {
    //    $(this).val("");
    //    $(this).change();
    //}
});

function disableBtn(elem) {
    var btn = $(elem).find("button[type=submit]");
    $(btn).attr("disabled", "disabled").addClass("btn-disabled");
}

function enableBtn(elem) {
    var btn = $(elem).find("button[type=submit]");
    $(btn).removeAttr("disabled").removeClass("btn-disabled");
}

function prepareTooltips() {
    $("[rel=tooltip], .tooltip").tooltip();
}

function clearValidationResult() {
    $(".field-validation-error").removeClass("field-validation-error").addClass("field-validation-valid").empty();
}

function showValidationResult(data) {
    clearValidationResult();
    //console.log(data);
    var props = data.properties;
    for (var key in props) {
        if (props[key] && props[key].length) {
            var msg = "<span for='" + key + "'>" + props[key][0] + "</span>";
            $(".field-validation-valid[data-valmsg-for=" + key + "],.field-validation-error[data-valmsg-for=" + key + "]").removeClass("field-validation-valid").addClass("field-validation-error").html(msg);
        }
    }
}

function clearForm(id) {
    clearFormElem('#' + id);
}

function clearFormElem(elem) {
    //console.log('cleaning element', elem);
    $(elem).find('input, textarea, select').removeClass('input-validation-error').val('');
    $(elem).find('img.clearonclose').attr("src", null);
}

function validateNumber() {
    $(document).on("keypress", ".number,input[data-val-number]", function (event, elem) {
        console.log("Number input key pressed");
        if (!event.charCode) return true;
        var key = event.which;
        if (key >= 48 && key <= 57 // 0-9
            || key === 44 //,(colon)
            || key === 45 //-Minus
            || key === 46 //.(Numpad dot)
        ) {
            return true;
        }
        return false;
    });
}

function callFn(fn, object, invoker) {
    //console.log('invoker:', invoker);
    //console.log('callback:', fn);
    //console.log('callback data:', object);
    if (typeof window[fn] === "function") {
        var func = window[fn];
        return func.call(null, object, invoker);
    }

    return null;
}

function getCrudFields(formId) {
    var form = $(formId);
    var arr = form.serializeArray();
    //arr = $.grep(arr, function (n) {
    //    return (n.value.length > 0);
    //});

    return arr;
}

// --------------------------
// Toastr - works
// --------------------------
function success(message) {
    toastr.success(message);
}
function error(message) {
    toastr.error(message);
}
function warning(message) {
    toastr.warning(message);
}
function info(message) {
    toastr.info(message);
}

function confirm(e, message, confirmCallback, cancelCallback, confirmButtonText, cancelButtonText, invoker, input, options) {
    confirmButtonText = confirmButtonText || "Yes"; //'Yes, delete it!'
    cancelButtonText = cancelButtonText || "No";
    var buttons = [];
    buttons.push({
        text: cancelButtonText,
        btnClass: "btn-danger"
    });
    buttons.push({
        text: confirmButtonText,
        btnClass: "btn-primary"
    });
    //console.log('confirm message: ', message);

    Swal.fire({
        title: 'Are you sure?',
        html: message || "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: confirmButtonText,
        cancelButtonText: cancelButtonText//,
        //customClass: 'swal-wide'
    }).then((result) => {
        if (result.value) {
            if ($.isFunction(confirmCallback)) {
                confirmCallback.call(null, invoker);
            }
        } else {
            if ($.isFunction(cancelCallback)) {
                cancelCallback.call(null, invoker);
            }
        }
        //if (result.value) {
        //    Swal.fire(
        //      'Deleted!',
        //      'Your file has been deleted.',
        //      'success'
        //    )
        //}
    })

    if (e) {
        e.preventDefault();
        e.stopPropagation();
    }
}

function loadSelectUrl(id, url, filterId, newVal) {
    if (url) {
        $('#' + id).attr('disabled', 'disabled');
        var val = $('#' + id).val();
        $.ajax({
            type: "get",
            url: url,
            data: { id: filterId === '' ? '0' : filterId },
            success: function (d) {
                if (d.success) {
                    loadSelect(id, d.values);
                    if (newVal) {
                        $('#' + id).val(newVal);
                    } else if (val) {
                        $('#' + id).val(val);
                    }
                } else if (d.error === "NotAuthorized") {
                    error("You are not authorized to complete this action!");
                } else {
                    error("Error");
                }

                $('#' + id).removeAttr('disabled');
            },
            error: function (err) {
                error(err);
                $('#' + id).removeAttr('disabled');
            },
            dataType: 'json'
        });
    }
}

function loadSelect(id, values) {
    var select = $('#' + id);
    var first = $(select).find('option').first();
    $(select).html('');

    var stringOptions = '';
    for (var i = 0; i < values.length; i++) {
        var item = values[i];
        var option = '<option value="' + item.value + '">' + item.text + '</option>';
        stringOptions += option;
    }

    $(select).append(first);
    $(select).append(stringOptions);
}

function loadAddress(data, prefix) {
    prefix = prefix || "AddressViewModel";
    var id = '#' + prefix;
    $(id + "_Id").val(data.Id);
    $(id + "_ProvinceId").val(data.ProvinceId);
    $(id + "_PostalCode").val(data.PostalCode);
    $(id + "_Address").val(data.Address);
    $(id + "_Address1").val(data.Address1);
    $(id + "_City").val(data.City);
}

function genericCallBack(data) {
    for (var name in data) {
        var ele = $("#" + name);
        if (ele) {
            $(ele).val(data[name]);
        }
    }
}

function createFilter(filter) {
    var container = '<div class="form-group col-md-4 control-filter">';
    var label = '<label class="control-label" for="id' + filter.name + '">' + filter.description + '</label>';
    var btnClose = '<button type="button" data-value="' + filter.name + '" class="close close-filter"><span aria-hidden="true">×</span><span class="sr-only">Cerrar</span></button>';
    var componen = '';
    var btn = '<div class="form-group col-md-12 clearfix btn-filter"><div class="pull-right"><button class="btn btn-danger" type="reset">Clear</button>&nbsp;&nbsp;<button class="btn btn-primary" type="submit">Search</button></div></div>';
    switch (filter.type) {
        case 1://textbox
            componen = '<input type="text" placeholder="' + filter.description + '" name="' + filter.name + '" id="id' + filter.name + '" class="form-control text-uppercase">';
            break;
        case 2://select
            componen = '<select name="' + filter.name + '" id="id' + filter.name + '" class="form-control"><option value="">-- Select --</option></select>';
            loadSelectUrl('id' + filter.name, filter.url);
            break;
        case 3://date
            componen = '<div class=""><div class="input-group date" id="txtDate" data-date-format="dd/MM/yyy">' +
                '<input type="text" id="id' + filter.name + '" name="' + filter.name + '" class="form-control" />' +
                '<span class="input-group-addon"><i class="fa fa-calendar"></i></span></div></div>';
            break;
        case 4://date Range
            container = '<div class="form-group col-md-8 control-filter">' + '<label class="control-label col-md-11" for="id' + filter.name + '">' + filter.description + '</label>' + btnClose +
                '<div class="control-group">' +
                '<div class="col-md-6">' +
                '<label class="" for="idfrom' + filter.name + '">Desde</label>' +
                '<div data-date-format="dd/MM/yyyy" id="from" class="input-group date">' +
                '<input type="text" class="form-control" name="from' + filter.name + '" id="idfrom' + filter.name + '">' +
                '<span class="input-group-addon"><i class="fa fa-calendar"></i></span>' +
                '</div></div>' +
                '<div class="col-md-6">' +
                '<label class=" " for="idto' + filter.name + '">Hasta</label>' +
                '<div data-date-format="dd/MM/yyyy" id="to" class="input-group date">' +
                '<input type="text" class="form-control" id="idTo' + filter.name + '" name="to' + filter.name + '">' +
                '<span class="input-group-addon">' +
                '<span class="fa-calendar fa"></span>' +
                '</span></div></div></div></div>';
            return container;
        default:

    }
    return container + label + btnClose + componen + '</div>' + btn;
}

function showNotifications(notifications) {
    if (notifications) {
        for (var i = 0; i < notifications.length; i++) {
            var notification = notifications[i];

            switch (notification.Type) {
                case 1:
                    success(notification.Message);
                    break;
                case 2:
                    warning(notification.Message);
                    break;
                case 3:
                    error(notification.Message);
                    break;
            }
        }
    }
}

function validateFileInput(input) {
    if ($(input).val().length === 0) return false;
    var allowedMimes = $(input).data("allowed-mimes");
    var allowedExts = $(input).data("allowed-exts");
    var file;
    if ($(input)[0].files)
        file = $(input)[0].files[0];
    if (file) {
        var type = file.type;
        var size = file.size;

        var reg = new RegExp(allowedMimes);
        if (!reg.test(type)) {
            error("This type of files is not allowed");
            return false;
        }

        if (size > 20971520) { //10240000
            error("This file exceeds the maximum allowed size");
            return false;
        }
    }
    else {
        var name = $(input).val();
        var extension = name.substring(name.lastIndexOf(".") + 1);

        reg = new RegExp(allowedExts);
        if (!reg.test(extension)) {
            error("This type of files is not allowed");
            return false;
        }
    }

    return true;
}

/*Date*/
function preparedDate() {
    //console.log("preparedDate");
    $('.date-from,.date-to,.date,.calendar').each(function () {
        var $this = $(this);
        var option = {
            useCurrent: false,
            //forceParse: false,
            //pickTime: false,
            //language: 'en',
            icons: {
                time: "fa fa-clock-o",
                date: "fa fa-calendar",
                up: "fa fa-arrow-up",
                down: "fa fa-arrow-down"
            },
            format: 'MM.DD.YYYY'
            //debug: true// 'YYYY/MM/DD',
            //        widgetPositioning: {
            //  horizontal: 'auto',
            //  vertical: 'auto'
            //},
            //showClose: true,
            //toolbarPlacement: 'bottom',
            //showTodayButton: true,
            //showClear: true,
        };
        var minDate = $this.data("min-date");
        var maxDate = new Date;//$this.data("max-date");

        if ($this.data("max-date")) {
            // If maxDate is set then set it
            maxDate = $this.data("max-date");
        } else {
            // else set new max date to current date + 1 year
            maxDate = new Date(maxDate.setMonth(maxDate.getMonth() + 12));
        }
        //console.log('minDate: ', minDate);
        //console.log('maxDate: ', maxDate);

        if (minDate)
            $.extend(option, { minDate: minDate });
        if (maxDate)
            $.extend(option, { maxDate: maxDate });
        $this.datetimepicker(option);
    });
}

function preparedMultiDate() {
    //console.log('preparedMultiDate');
    $('.mdate').each(function () {
        var $this = $(this);
        var options = {
            autoclose: true,
            forceParse: false,
            todayHighlight: true,
            format: 'mm.dd.yyyy',
            multidate: true,
            startDate: '01.01.1900',
            endDate: '01.01.2100' //new Date
        };
        $this.datepicker(options).on('hide', function (e) {
            e.stopPropagation();
        });
    });
}

function preparedHtmlEditor() {
    if ($.fn.ckeditor)
        $("textarea.html-editor").ckeditor();
}

//function preparedSummernoteEditor() {
    //console.log("preparedSummernoteEditor");
    //$("textarea.summernote").summernote();
//}

function prepareValidators(selector) {
    var form = $(selector).removeData("validator").removeData("unobtrusiveValidation");
    jQuery.validator.unobtrusive.parse(form);
    var validator = form.data('validator');
    if (validator && validator.settings)
        validator.settings.ignore = "";
}

function validateTabForm(parent, tabSelector) {
    var form = $(parent);
    var valid = $(form).valid();
    if (!valid) {
        var firstError = $(form).find(".input-validation-error:first");
        if (firstError && firstError.length) {
            var tabId = $(firstError).closest(".tab-pane").prop("id");
            $(tabSelector + ' a[href="#' + tabId + '"]').tab('show');
        }
    }
    return valid;
}

function makeUrl(url, param, value) {
    var makeParam = '?' + param + '=' + value;
    var index = url.indexOf('?');
    var newUrl = url;
    if (index !== -1) {
        newUrl = url.substring(0, index);
    }
    newUrl += makeParam;
    return newUrl;
}

function prepareCheckboxes(isStepsCheck) {
    //console.log("prepareCheckboxes iCheck!!!");

    if (isStepsCheck) {
        $('.i-checks-steps').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
            //checkboxClass: 'icheckbox_minimal',
            //radioClass: 'iradio_minimal',
            increaseArea: '20%' // optional
        });
    } else {
        $('.i-checks').iCheck({
            checkboxClass: 'icheckbox_square-green',
            radioClass: 'iradio_square-green',
            //checkboxClass: 'icheckbox_minimal',
            //radioClass: 'iradio_minimal',
            increaseArea: '20%' // optional
        });
    }



    //$('.i-checks').iCheck({
    //    checkboxClass: 'icheckbox_square-green',
    //    radioClass: 'iradio_square-green',
    //    //checkboxClass: 'icheckbox_minimal',
    //    //radioClass: 'iradio_minimal',
    //    increaseArea: '20%' // optional
    //});


    ////$("input[type='checkbox'], input[type='radio']").iCheck({
    ////    checkboxClass: 'icheckbox_minimal',
    ////    radioClass: 'iradio_minimal'
    ////});
}

function prepareSelect2() {
    //console.log("prepareSelect2: Tumepitia!!!");
    if ($('.select-data,.select-data-ajax').length === 0) {
        return;
    }

    var options = {
        minimumResultsForSearch: 1,
        //theme: 'bootstrap4',
        "data-placeholder": "Select options",
        tags: true,
        insertTag: function (data, tag) {
            // Insert the tag at the end of the results
            data.push(tag);
        }
    };

    if ($('.select-data').length) {
        $(".select-data").each(function () {
            $(this).select2(options).on('change', function () {
                $(this).valid();
            }).valid();

            $(this).on("select2:opening", function (e) {
                select2Opening(this);
            });
        });
    }

    if ($('.select-data-ajax').length) {
        $(".select-data-ajax").each(function() {
            var url = $(this).data('action');
            $.extend(options, {
                ajax: {
                    url: url,
                    dataType: 'json',
                    delay: 250,
                    data: function(params) {
                        return {
                            term: params.term, // search term
                            page: params.page
                        };
                    },
                    processResults: function(data, page) {
                        // parse the results into the format expected by Select2.
                        // since we are using custom formatting functions we do not need to
                        // alter the remote JSON data
                        return {
                            results: data
                        };
                    },
                    cache: true
                },
                escapeMarkup: function(markup) { return markup; }, // let our custom formatter work
                minimumInputLength: 3
            });
            $(this).select2(options).on('change', function() {
                $(this).valid();
            }).valid();

            $(this).on("select2:opening", function(e) {
                select2Opening(this);
            });
        });
    }
}

function select2Opening(elem) {
    if ($(elem).closest(".modal-dialog").length) {
        $("body > .select2-container").addClass("select-in-modal");
    } else {
        $("body > .select2-container").removeClass("select-in-modal");
    }
}

//window.addEventListener('wheel', e => e.preventDefault(), {
//    passive: false
//});
//$(document).on("mousewheel", ".chosen-select", function (e) {

//    //e.cancelable && e.preventDefault()
//    //e.preventDefault();
//    //console.log("Acha ufala");
//}, { passive: false });

//document.addEventListener("mousewheel", this.mousewheel.bind(this), { passive: false });


//handlerList = document.getElementsByClassName("sortable-handler");
//for (var i = 0, len = handlerList.length | 0; i < len; i = i + 1 | 0) {
//    handlerList[i].style.style.touchAction = "none";
//}


function prepareChosenSelect(isStepsChosen) {
    //console.log("Preparing chosen select class");
    //$('.chosen-select,.chosen-select-results')

    var options = {
        disable_search_threshold: 1,
        //inherit_select_classes: true,
        width: '100%'
    };

    if (isStepsChosen) {
        if ($('.chosen-select-steps').length) {
            $('.chosen-select-steps').each(function () {
                $(this).chosen(options);
            });
        }
    } else {
        if ($('.chosen-select').length) {
            $('.chosen-select').each(function () {
                $(this).chosen(options);
            });
        }
    }
}

/**
 * Returns a random integer between min(inclusive) and max(inclusive).
 *
 * @param {number} min Lower limit number
 * @param {number} max Upper limit number
 * @returns {number} A random integral
 */
function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

function trapFocus(elementId) { //, num = 0) {
    var element = document.querySelector(elementId);
    //console.log('focusable element:', element);
    var focusableEls = element.querySelectorAll('a[href]:not([disabled]), button:not([disabled]), textarea:not([disabled]), input[type="text"]:not([disabled]):not([readonly]), input[type="number"]:not([disabled]):not([readonly]), input[type="radio"]:not([disabled]), input[type="checkbox"]:not([disabled]), select:not([disabled]):not([readonly]), checkbox:not([disabled])');
    var firstFocusableEl = focusableEls[0]; //[num];
    var lastFocusableEl = focusableEls[focusableEls.length - 1];

    //console.log('focusable elements:', focusableEls);    
    if (firstFocusableEl) {
        setTimeout(function () {
            //let elementId = firstFocusableEl.id;
            if (firstFocusableEl.id) {
                //console.log('elementId:', elementId);
                if ($('#' + firstFocusableEl.id).hasClass('chosen-select')) {
                    //console.log('has chosen-select class', firstFocusableEl.id);
                    $('#' + firstFocusableEl.id).trigger('chosen:activate');
                } else if ($('#' + firstFocusableEl.id).hasClass('date')) {
                    // if fisrt element is date element dont focus, do nothing
                } else {
                    firstFocusableEl.focus();
                }
            } else {
                firstFocusableEl.focus();
            }
        }, 200);

        // This is what we had here
        //firstFocusableEl.focus();
    }

    //console.log('firstFocusableEl:', firstFocusableEl);
    //console.log('lastFocusableEl:', lastFocusableEl);

    var KEYCODE_TAB = 9;

    element.addEventListener('keydown', function (e) {
        var isTabPressed = (e.key === 'Tab' || e.keyCode === KEYCODE_TAB);

        if (!isTabPressed) {
            return;
        }

        if (e.shiftKey) /* shift + tab */ {            
            if (document.activeElement === firstFocusableEl) {
                lastFocusableEl.focus();
                //console.log('shiftKey lastFocusableEl:',lastFocusableEl);
                e.preventDefault();
            }
        } else /* tab */ {
            //console.log('tab document.activeElement:', document.activeElement);
            if (document.activeElement === lastFocusableEl) {
                let elementId = firstFocusableEl.id;
                //console.log('has chosen-select class', elementId);

                if (!elementId) {
                    firstFocusableEl.focus();
                } else {
                    //console.log('elementId2:', elementId);
                    if ($('#' + elementId).hasClass('chosen-select')) {
                        //console.log('has chosen-select class', elementId);
                        $('#' + elementId).trigger('chosen:activate');
                    } else {
                        firstFocusableEl.focus();
                    }
                }

                //firstFocusableEl.focus();
                e.preventDefault();
            }
        }
    });
}

function getIndex(array, value) {
    for (var i = 0; i < array.length; i++) {
        //console.log('array[0]: ', array[0]);
        if (array[i] === value) {
            return i;
        }
    }
    return -1;
}

function inConstruction() {
    return info("Functionality in construction!");
}

function reloadPage() {
    setTimeout(function () {
        window.location.reload();
    }, 3000);
}

$(document).on("click", "a.delete-app-file:not(.disabled)", function () {
    var $element = $(this),
        url = $element.data('action'),
        id = $element.data('id'),
        callback = $element.data('callback'),
        message = $element.data('message');

    var cont = function (data) {
        callFn(callback, data, $element);
    };

    confirm(null, message, function () {
        $.ajax({
            method: 'DELETE',
            url: url,
            data: {
                'id': id
            },
            success: function (res) {
                if (res.success) {
                    success(res.message);
                    cont(res);
                } else if (res.error === "NotAuthorized") {
                    error("You are not authorized to complete this action!");
                } else {
                    error(res.message || "Oops, an error occured, please try again!");
                }
            },
            error: function (res) {
                error(res.message);
            }
        })
    });
});

$(document).on("click", "button.toggle-status:not(.disabled)", function () {
    var $element = $(this),
        url = $element.data('action'),
        id = $element.data('id'),
        dTables = $element.data("table") ? $element.data("table").split(' ') : undefined,
        callback = $element.data('callback'),
        message = $element.data('message');

    var cont = function (data) {
        if (dTables !== undefined) {
            if (dTables && dTables !== '') {
                for (var i = 0; i < dTables.length; i++) {
                    $('#' + dTables[i]).DataTable().ajax.reload();
                }
            }
        }
        callFn(callback, data, $element);
    };

    confirm(null, message, function () {
        $.ajax({
            method: 'POST',
            url: url,
            data: { 'id': id },
            dataType: 'json',
            success: function (res) {
                if (res.success) {
                    success(res.message);
                    cont(res);
                } else if (res.error === "NotAuthorized") {
                    error("You are not authorized to complete this action!");
                } else {
                    error(res.message || "Oops, an error occured, please try again!");
                }
            },
            error: function (res) {
                error(res.message);
            }
        })
    });
});

// If state is true enable else disable
function toggleSelectEnabledState(inputId, boolEnable) {
    if (boolEnable) {
        //console.log('toggleSelectEnabledState1', boolEnable)
        $('#' + inputId).css('pointer-events', 'auto');
        $('#' + inputId).prop('disabled', false).trigger("chosen:updated");
    } else {
        //console.log('toggleSelectEnabledState2', boolEnable)
        $('#' + inputId).css('pointer-events', 'none');
        $('#' + inputId).val('').trigger('chosen:updated');
        //So chosen is disabled but value from select is posted.
        $('#' + inputId).prop('disabled', true).trigger('chosen:updated');//.prop('disabled', false);
    }
}

// If state is true enable else disable
function toggleCheckBoxCheckedState(inputId, boolCheck) {
    if (boolCheck) {
        //console.log('toggleCheckBoxCheckedState', boolCheck)
        $('#' + inputId).css('pointer-events', 'auto');
        $('#' + inputId).prop('disabled', false);
    } else {
        //console.log('toggleCheckBoxCheckedState', boolCheck)
        $('#' + inputId).prop('checked', false).iCheck('update');
        $('#' + inputId).prop('disabled', true);
        $('#' + inputId).css('pointer-events', 'none');
    }
}

//// select 1 option and deselect the same option in another select menu
//$(".selectKeyword").chosen().change(function () {
//    var selectedValue = $(this).find('option:selected').val();
//    $(".selectKeyword").find('option[value="' + selectedValue + '"]:not(:selected)').attr('disabled', 'disabled');
//    $(".selectKeyword").trigger("chosen:updated");
//});
