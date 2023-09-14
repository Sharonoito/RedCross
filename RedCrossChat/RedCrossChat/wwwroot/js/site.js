
$.ajaxSetup({
    headers: {
        'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
    }
});

$(document).on("shown.bs.modal", ".modal", function () {

});

$(document).on('ready', function () { //converts input value to two decimal places
    $("input.app-cash").val(parseFloat($("input.app-cash").val().trim()).toFixed(2))
})

function error(message) {
    toastr.error(message);
    // console.log('error log', message);

}
function success(message, then = null) {
    toastr.success(message);

    if (then != null)
        then()
    // console.log("sucess log",message)
}

function info(message) {
    toastr.info(message)
}


function showInformation(message) {
    // console.log(message)
}

function disableBtn(elem) {
    var btn = $(elem).find("button[type=submit]");
    $(btn).attr("disabled", "disabled").addClass("btn-disabled");

    console.log("submit-button", btn)
}

function enableBtn(elem) {
    var btn = $(elem).find("button[type=submit]");
    $(btn).removeAttr("disabled").removeClass("btn-disabled");
}

function callFn(fn, object, invoker) {
    if (typeof window[fn] === "function") {
        var func = window[fn];
        return func.call(null, object, invoker);
    }
    return null;
}

function getCrudFields(formId) {
    var form = $(formId);
    var arr = form.serializeArray();
    return arr;
}


function showValidationError(inputId, msg) {
    // Check if it is a select2 input
    if ($('#' + inputId).hasClass('select-data') || $('#' + inputId).hasClass('select2')) {
        $('#' + inputId).next().find('.select2-selection').addClass('input-validation-error');
    } else {
        $(".input-validation-valid[name=" + inputId + "],.input-validation-error[name=" + inputId + "]").removeClass("input-validation-valid").addClass("input-validation-error"); //.html(msg);
    }
    $(".field-validation-valid[data-valmsg-for=" + inputId + "],.field-validation-error[data-valmsg-for=" + inputId + "]").removeClass("field-validation-valid").addClass("field-validation-error").html(msg);
}

function clearValidationError(inputId) {
    if ($('#' + inputId).hasClass('select-data') || $('#' + inputId).hasClass('select2')) {
        $('#' + inputId).next().find('.select2-selection').removeClass('input-validation-error');
    } else {
        $(".input-validation-valid[id=" + inputId + "],.input-validation-error[id=" + inputId + "]").removeClass("input-validation-error").addClass("input-validation-valid");
    }
    $(".field-validation-valid[data-valmsg-for=" + inputId + "],.field-validation-error[data-valmsg-for=" + inputId + "]").removeClass("field-validation-error").addClass("field-validation-valid").html('');
}

function clearValidationResult() {
    $(".is-invalid").removeClass("is-invalid");
    $(".error").empty();
    $(".input-validation-error").removeClass("input-validation-error").addClass("input-validation-valid");
}

function showValidationResult(data) {
    clearValidationResult();

    console.log('validation data', data);

    var props = data.responseJSON.errors;
    console.log('validation props:', props);
    for (var key in props) {
        console.log('validation key:', key);
        console.log('validation element:', props[key]);
        if (props[key] && props[key].length) {
            var msg = "<span for='" + key + "'>" + props[key] + "</span>";
            console.log('validation msg:', msg);

            $("#" + key).next().html(msg);
            $("#" + key).addClass("is-invalid");
        }
    }
}

$(document).on("click", ".load-modal", function () {
   
    const beforeLoad = $(this).data("beforeload");

    if (beforeLoad) {
        callFn(beforeLoad, this);
    }

    const $element = $(this),
        modal = $element.data("modal") || $element.closest("ul").data("modal"),
        url = $element.data('action'),
        id = $element.data('id'),
        hiddenId = $element.data('hidden'),
        callback = $element.data('callback'),
        viewUrl = $element.data('viewurl'),
        content = $element.data('content') ? $element.data('content') : ".modal-content";

    let obj = {};


    console.log('load-modal - Here', modal);
    console.log('load-modal - Here', modal);
    if ($element.data('post') != "" || $element.data('post') != "undefined") {

        //  console.log($element.data('post'))

        //console.log(atob($element.data('post')))

        //  console.log(JSON.parse(atob($element.data('post'))))
        //  obj=JSON.parse(atob($element.data('post')));
    }

    $("#" + modal).data("invoker", $element);

    if (hiddenId) {
        $("#" + hiddenId).val(id);
    }

    let data = { 'id': id };

    let dataType = "json";

    if (viewUrl) {
        data = "";
        dataType = "html";
    }

    if (url) {
        $.ajax({
            type: "get",
            url: url,
            data: data,
            cache: true, // Added this to clear timestamp added to the url
            dataType: dataType,
            success: function (response) {
                console.log('response: ', response);
                if (viewUrl) {
                    try {
                        var obj = JSON.parse(response);
                        if (obj.success === false) {
                            //console.log('obj.message: ', obj.message);

                            setTimeout(function () {
                                $('#' + modal).modal("hide");
                            }, 500);

                            //$('#' + modal).modal("hide");
                            error(obj.message || "Error");
                            return;
                        }
                    } catch (e) {
                        // do nothing, it's html content
                    }


                    $('#' + modal + " .modal-content").html(response);

                    if (hiddenId) {
                        $("#" + hiddenId).val(id);
                    }

                    //jQuery.validator.unobtrusive.parse('#' + modal);
                    //var formid = $('#' + modal).find('form').attr('id');
                    //$.validator.unobtrusive.parse('#' + formid);

                    if (callback)
                        callFn(callback, id);

                } else {
                    if (callback)
                        callFn(callback, response);
                }

                if (modal) {
                    $('#' + modal).modal();
                }
            },
            error: function (response) {
                // console.log('response: ', response);
                if (response.status === 401) {
                    // error(response.responseText);
                    setTimeout(function () {
                        $('#' + modal).modal("hide");
                    }, 500);
                } else {
                    error("An error occured.");
                }
            }
        });
    } else {

        if (modal) {
            $('#' + modal).modal('show');
        }

        callFn(callback, id);
    }

    return false;
});

$(document).on("submit", "form.modal-ajax", function (e) {

    try {
        e.preventDefault();

        let $form = $(this);

        disableBtn($form);

        let callInit = $form.data("callinit");

        if (callInit && callInit !== '') {
            callFn(callInit);
        }

        const data = getCrudFields($form),
            dTables = $form.data("table") ? $form.data("table").split(' ') : undefined,
            modal = $form.data("modal").toString().trim(),
            url = $form.attr('action'),
            callback = $form.data("callback") ? $form.data("callback").split(' ') : undefined;


        let cont = function () {
            if (dTables !== undefined) {
                if (dTables && dTables !== '') {
                    for (let i = 0; i < dTables.length; i++) {

                        if (dTables[i] !== '') {
                            // console.log("dtable",dTables[i])
                            $('#' + dTables[i].toString().trim()).DataTable().ajax.reload();
                        }


                    }
                }
            }

            if (callback !== undefined)
                callback.forEach(call => {
                    callFn(call)
                })

            console.log("submitFunction", callback)

            if (callback && callback !== '') {
                //callFn(callback, data, $("#" + modal).data("invoker"));
                callFn(callback);
            }



            if (modal && modal !== '') {
                $('#' + modal).modal('hide');
            }
        };

        if (url && url !== '') {
            requestData = null;
            $.ajax({
                type: "POST",
                url: url,
                data: data,
                dataType: 'json',
                success: function (response) {
                    enableBtn($form);
                    if (response.success === true) { // OK
                        clearValidationResult();
                        success(response.message || "Saved successfully!!");
                        setTimeout(function () {
                            $('#' + modal).modal("hide");
                        }, 500);
                        if (response.requestData) {
                            requestData = response.requestData;
                        }
                        cont();
                        if (response.redirect) {
                            redirect(response.redirect);
                        }
                    }
                },
                error: function (response) {
                    // console.log("response: ", response);
                    enableBtn($form);
                    if (response.status === 401) {
                        setTimeout(function () {
                            $('#' + modal).modal("hide");
                        }, 500);
                    }
                    else if (response.status === 422) { // error de validacion
                        error('Invalid form inputs.');
                        showValidationResult(response);
                    } else { // error del servidor
                        if (response == undefined || response.message == undefined) {
                            // console.error("Something Broke !")
                        } else error(response.message.length === 0 ? "There was an error please retry!" : response.message);
                    }
                },
            });
        } else {
            cont();
        }
        return false;
    }
    catch (err) {
        // console.log("Catched: ", err.message);
    }
});

/** AJAX FUNCTIONS
 *
 */
let isLoading = false;
$(document).ajaxSend(function (e, response, options) {
    let preventLoading = false;
    if (!isLoading) {
        preventLoading = true;
        isLoading = true;
    } else {
        if (options.url) {
            if (options.url.toString().indexOf("sidx") !== -1) {
                preventLoading = true;
            }
        }
    }
    if (!preventLoading) {
        $("#loaders").show();
        $("#loaders").animate({ "right": "0px" }, "slow");
    }
});
$(document).ajaxComplete(function (event, xhr, options) {
    $("#loaders").animate({ "right": "-300px" }, 1000, function () {
        $("#loaders").hide();
    });
    // console.log("response: ", xhr);
    if (xhr && xhr.Data && xhr.Data.LogOnUrl) {
        if (xhr.status === 401) {
            // window.location.href = xhr.Data.LogOnUrl;
            error("You are not authorised to perform this operation.")
            return;
        }
    } else if (xhr.status === 401) {
        error(xhr.responseText);
        return;
    }

});


function confirm(message, confirm = null, cancel = null, confirmText = null) {
    Swal.fire({
        title: "Are you sure?",
        text: message,
        icon: "warning",
        showCancelButton: !0,
        confirmButtonText: confirmText ?? "Yes, delete it!",
        customClass: { confirmButton: "btn btn-primary me-3", cancelButton: "btn btn-label-secondary" },
        buttonsStyling: !1,
    }).then(function (t) {

        if (t.isConfirmed && confirm != null)
            confirm()

        if (t.isDismissed && cancel != null)
            cancel()

    });
}
$.fn.dataTable.ext.errMode = function (settings, helpPage, message) {

    console.log("Datatable settings")

    if (message.includes("Requested unknown parameter")) {
        info("Parameter not Found : " + message.substring(
            message.indexOf("'") + 1,
            message.lastIndexOf("'")
        ));
    } else
        error("Couldn't load data at this moment.")
};

function jsSimpleTable(id, url, columns, buttons, pageLength = 10, data = {}) {

    const jsDataTable = {
        table: null,
        initializeDataTable: function () {
            jsDataTable.table = $('#' + id).DataTable({
                processing: true,
                serverSide: true,
                serverMethod: 'post',
                ajax: {
                    url: url,
                    data: data
                },
                pageLength: pageLength,
                columns: columns,
                columnDefs: [{
                    render: buttons,
                    targets: -1,
                    orderable: false
                }],
                order: [0, 'asc'],
                language: {
                    processing: 'Loading records... Please wait.',
                },
                pagingType: 'full_numbers',
                oLanguage: {
                    oPaginate: {
                        sNext: '>',
                        sPrevious: '<',
                        sFirst: '<<',
                        sLast: '>>'
                    }
                },
                responsive: true,
            });
        },
        getData: function () {
            if (jsDataTable.table == null) {
                jsDataTable.initializeDataTable();
            } else {
                jsDataTable.table.ajax.reload();
            }
        }

    }

    return jsDataTable
}

function jsNestedTable(id, url, columns, buttons, pageLength = 10, data = {}) {

    const jsDataTable = {
        table: null,
        initializeDataTable: function () {
            jsDataTable.table = $('#' + id).DataTable({
                processing: true,
                serverSide: true,
                serverMethod: 'post',
                ajax: {
                    url: url,
                    data: data
                },
                pageLength: pageLength,
                columns: columns,
                columnDefs: [{
                    render: buttons,
                    targets: -1,
                    orderable: false
                }],
                order: [0, 'asc'],
                language: {
                    processing: 'Loading records... Please wait.',
                },
                pagingType: 'full_numbers',
                oLanguage: {
                    oPaginate: {
                        sNext: '>',
                        sPrevious: '<',
                        sFirst: '<<',
                        sLast: '>>'
                    }
                },
                responsive: true,
            });
        },
        getData: function () {
            if (jsDataTable.table == null) {
                jsDataTable.initializeDataTable();
            } else {
                jsDataTable.table.ajax.reload();
            }
        }

    }

    return jsDataTable
}
