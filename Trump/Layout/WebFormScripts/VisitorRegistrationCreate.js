$("#accordion").accordion({
    icons: null,
    heightStyle: "content"
});



$("#DateOfBirthString").datepicker({
    showOn: "",
    buttonImage: "../../images/calendar.gif",
    buttonImageOnly: true,
    dateFormat: 'mm/dd/yy',
    changeMonth: true,
    changeYear: true,
    yearRange: '1900:2100'
});


$("#DateOfBirthString").click(function () {
    $("#DateOfBirthString").datepicker("show");
});

//$("#IDProofValidityStr").datepicker({
//    showOn: "",
//    buttonImage: "../../images/calendar.gif",
//    buttonImageOnly: true,
//    dateFormat: 'mm/dd/yy',
//    changeMonth: true,
//    changeYear: true,
//    yearRange: '1900:2100'
//});


//$("#IDProofValidityStr").click(function () {
//    $("#IDProofValidityStr").datepicker("show");
//});

  
$("#VisitorCompanyID").change(function () {
    var Value = this.options[this.selectedIndex].value;
    if (Value > 0) {
        var Name = $("#VisitorCompanyID option:selected").text();
        $('#VisitorCompanyName').val(Name);
    }
    else {
        $('#VisitorCompanyName').val('');
    }
});

$("#VisitorTypeID").change(function () {
    var Value = this.options[this.selectedIndex].value;
    if (Value > 0) {
        var Name = $("#VisitorTypeID option:selected").text();
        $('#VisitorTypeName').val(Name);
    }
    else {
        $('#VisitorTypeName').val('');
    }
});

$("#RemoveImage").click(function () {

    $('#preview-iframe').contents().find('img').attr('src', '');

    document.getElementById('imgVisitorPhoto').src = '';
    document.getElementById('imgVisitorPhotoZoom').src = '';

    $('#divVisitorPhoto').hide();

});

//Chandni M:Visitor Registration Request :10th April,2014
$("#RemoveIDImage").click(function () {

    $('#preview-iframe').contents().find('img').attr('src', '');

    document.getElementById('imgVisitorIDPhoto').src = '';
    document.getElementById('imgVisitorIDPhotoZoom').src = '';

    $('#divVisitorIDPhoto').hide();

});

function CloseWindow(SelectedValue) {
    if (window.dialogArguments && !window.dialogArguments.closed) {
        if (window.dialogArguments.location.toString().toLowerCase().indexOf("quickaddvisitor") != -1
                || window.dialogArguments.location.toString().toLowerCase().indexOf("create") != -1
                || window.dialogArguments.location.toString().toLowerCase().indexOf("edit") != -1) {
            window.dialogArguments.GetVisitorsByVisitorCompany(SelectedValue);
        }
    }

    self.close();
}


//Chandni M:Visitor Registration Request :10th April,2014
function ChangeImage(ImgValue) {
    
    if (ImgValue.toString() == 'imgVisitorPhoto') {
        return filePreview();
    }
    else if (ImgValue.toString() == 'imgVisitorIDPhoto') {
        return filePreviewID();
    }
   

}

function GetCapturedPhoto() {
    
    $.ajax({
        cache: false,
        type: "POST",
        async: false,
        url: ITX3ResolveUrl('VisitorRegistration/GetPreviewImage'),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            
            if (data != '') {

                document.getElementById('imgVisitorPhoto').src = data;
                document.getElementById('imgVisitorPhotoZoom').src = data;
                $('#imgVisitorPhoto').show();
                $('#imgVisitorPhotoZoom').show();

                $('#divVisitorPhoto').show();
            }
            else {
                $('#divVisitorPhoto').hide();
            }
        },

        error: function (xhr, ret, e) {
            
        }

    });
}

function GetCapturedIDPhoto() {
    $.ajax({
        cache: false,
        type: "POST",
        async: false,
        url: ITX3ResolveUrl('VisitorRegistration/GetPreviewIDImage'),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data != '') {

                document.getElementById('imgVisitorIDPhoto').src = data;
                document.getElementById('imgVisitorIDPhotoZoom').src = data;

                $('#divVisitorIDPhoto').show();
            }
            else {
                $('#divVisitorIDPhoto').hide();
            }
        },
        error: function (xhr, ret, e) {
        }

    });
}

var filePreview = function () {
    

    window.callback = function () { };

    if ($('body').html().toLowerCase().indexOf("preview-iframe") <= 0) {
        $('body').append('<iframe id="preview-iframe" onload="callback();" name="preview-iframe" style="display:none" />');
    }

    var action = $('#frmIndexVisiterRegistration').attr('target', 'preview-iframe').attr('action');

    $('#frmIndexVisiterRegistration').attr('action', '/VisitorRegistration/PreviewImage');

    window.callback = function () {
        var result = $('#preview-iframe').contents().find('img').attr('src');
        document.getElementById('imgVisitorPhoto').src = result;
        document.getElementById('imgVisitorPhotoZoom').src = result;
        
        if (result.length > 0) {

            $('#imgVisitorPhoto').show();
            $('#imgVisitorPhotoZoom').show();
            $('#divVisitorPhoto').show();
        }
        else {
            $('#imgVisitorPhoto').hide();
            $('#imgVisitorPhotoZoom').hide();
            $('#divVisitorPhoto').hide();
        }

    };


    $('#frmIndexVisiterRegistration').submit().attr('action', action).attr('target', '');


};

//Chandni M : Visitor Registration Request Form : 15 April,2014
var filePreviewID = function () {

    window.callback = function () { };

    if ($('body').html().toLowerCase().indexOf("preview-iframe1") <= 0) {
        $('body').append('<iframe id="preview-iframe1" onload="callback();" name="preview-iframe1" style="display:none" />');
    }

    var action = $('#frmIndexVisiterRegistration').attr('target', 'preview-iframe1').attr('action');

    $('#frmIndexVisiterRegistration').attr('action', '/VisitorRegistration/PreviewImageID');

    window.callback = function () {
        var result = $('#preview-iframe1').contents().find('img').attr('src');
        document.getElementById('imgVisitorIDPhoto').src = result;
        document.getElementById('imgVisitorIDPhotoZoom').src = result;

        if (result.length > 0) {

            $('#imgVisitorIDPhoto').show();
            $('#imgVisitorIDPhotoZoom').show();
            $('#divVisitorIDPhoto').show();
        }
        else {
            $('#imgVisitorIDPhoto').hide();
            $('#imgVisitorIDPhotoZoom').hide();
            $('#divVisitorIDPhoto').hide();
        }

    };


    $('#frmIndexVisiterRegistration').submit().attr('action', action).attr('target', '');


};

