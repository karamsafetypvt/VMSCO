



//Change image on browse


function ChangeImagefv() {
    return filePreviewFV();
}
function ChangeImageVG() {
    return filePreviewVG();
}
function ChangeImagePT() {
    return filePreviewPT();
}
//Image browse
var filePreviewFV = function () {


    window.callback = function () { };
    if ($('body').html().toLowerCase().indexOf("preview-iframe") <= 0) {
        $('body').append('<iframe id="preview-iframe" onload="callback();" name="preview-iframe" style="display:none" />');
    }
    debugger;
    var action = $('#FormQuickAddVisitor').attr('target', 'preview-iframe').attr('action');
    //$('form').get(3).setAttribute('action', '/Visitor/PreviewImage');
    $('form[id="FormQuickAddVisitor"]')[0].setAttribute('action', '/Visitor/PreviewImage');
    window.callback = function () {

        var result = $('#preview-iframe').contents().find('img').attr('src');

        document.getElementById('imgVisitorPhotofv').src = result;
        //document.getElementById('imgVisitorPhotoZoomfv').src = result;
        if (result != undefined) {
            if (result.length > 0) {

                $('#divVisitorPhotofv').show();
                $('#imgVisitorPhotofv').show();
                //$('#imgVisitorPhotoZoomfv').show();
            }
            else {
                $('#divVisitorPhotofv').show();
                $('#imgVisitorPhotofv').show();
                //$('#imgVisitorPhotoZoomfv').show();
            }
        }

    };

    $('form[id="FormQuickAddVisitor"]')[0].submit();
    //$('form')[3].submit()

};


//Image browse
var filePreviewVG = function () {

    window.callback = function () { };
    if ($('body').html().toLowerCase().indexOf("preview-iframe") <= 0) {
        $('body').append('<iframe id="preview-iframe" onload="callback();" name="preview-iframe" style="display:none" />');
    }
    var action = $('#frmAppointmentPassPrint').attr('target', 'preview-iframe').attr('action');
    $('#frmAppointmentPassPrint').attr('action', '/VisitorGatepass/PreviewImage');

    window.callback = function () {

        var result = $('#preview-iframe').contents().find('img').attr('src');
        //$('#imgVisitorPhoto').src = result;
        document.getElementById('imgVisitorPhoto').src = result;
        //document.getElementById('imgVisitorPhotoZoomfv').src = result;
        if (result != undefined) {
            if (result.length > 0) {

                //$('#divPhoto').show();
                $('#imgVisitorPhoto').show();
                //$('#imgVisitorPhotoZoom').show();
            }
            else {
                //$('#divVisitorPhotofv').show();
                $('#imgVisitorPhoto').show();
                //$('#imgVisitorPhotoZoomfv').show();
            }
        }

    };


    $('#frmAppointmentPassPrint').submit().attr('action', action).attr('target', '');

};

//Image browse
var filePreviewPT = function () {

    window.callback = function () { };
    if ($('body').html().toLowerCase().indexOf("preview-iframe") <= 0) {
        $('body').append('<iframe id="preview-iframe" onload="callback();" name="preview-iframe" style="display:none" />');
    }
    var action = $('#frmPassTemplate').attr('target', 'preview-iframe').attr('action');
    $('#frmPassTemplate').attr('action', '/PassTemplate/PreviewImage');

    window.callback = function () {

        var result = $('#preview-iframe').contents().find('img').attr('src');
        //$('#imgVisitorPhoto').src = result;
        debugger;
        document.getElementById('imgPhoto').src = result;
        //document.getElementById('imgVisitorPhotoZoomfv').src = result;
        if (result != undefined) {
            if (result.length > 0) {

                $('#divPhoto').show();
                $('#imgPhoto').show();
                //$('#imgVisitorPhotoZoom').show();
            }
            else {
                $('#divPhoto').show();
                $('#imgPhoto').show();
                //$('#imgVisitorPhotoZoomfv').show();
            }
        }

    };


    $('#frmPassTemplate').submit().attr('action', action).attr('target', '');

};


$("#RemoveImagefv").click(function () {

    $('#preview-iframe').contents().find('img').attr('src', '');

    var VisitorID = $('#VisitorID').val();

    if (VisitorID > 0) {

        $.ajax({
            cache: false,
            type: "POST",
            async: true,
            url: ITX3ResolveUrl('Visitor/RemoveVisitorImage'),
            data: JSON.stringify({ visitorID: VisitorID }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.Result == 'OK') {
                    document.getElementById('imgVisitorPhotofv').src = '';
                    document.getElementById('imgVisitorPhotoZoomfv').src = '';
                    $('#divVisitorPhoto').hide();
                }
                else {
                    $('<div title="Information"></div>').dialog({
                        open: function (event, ui) {
                            $(this).html(data.Message);
                        }
                    });
                }
            },
            error: function (xhr, ret, e) {
            }

        });

    }
    else {
        document.getElementById('imgVisitorPhotofv').src = '';
        document.getElementById('imgVisitorPhotoZoomfv').src = '';
        $('#divVisitorPhotofv').hide();
    }
});



//Chandni M  : Regarding PopUI Changes : 11/07/2014
function QuickAddCategory(CategoryType) {
    debugger;
    $('#MessageCategoryForm').css('display', 'none')
    $('#MessageCategoryForm').text('');
    var flag = true;
    var CategoryValue = '';
    var CategoryDetailValue = '';
    var CategoryDetailValue1 = '';
    var SiteId = '';

    if (CategoryType == '1') {
        CategoryValue = $('#VisitorTypeCategoryValue').val();
    }
    else if (CategoryType == '2') {
        CategoryValue = $('#CityCategoryValue').val();
    }
    else if (CategoryType == '3') {
        CategoryValue = $('#StateCategoryValue').val();
    }
    else if (CategoryType == '4') {
        CategoryValue = $('#CountryCategoryValue').val();
    }
    else if (CategoryType == '5') {
        CategoryValue = $('#MeetingLocationCategoryValue').val();
        SiteId = $('#SiteID').val();
    }
    else if (CategoryType == '6') {
        CategoryValue = $('#DepartmentCategoryValue').val();
        SiteId = $('#SiteID').val();
    }
    else if (CategoryType == '7') {
        CategoryValue = $('#VisitTypeCategoryValue').val();
    }
    else if (CategoryType == '8') {
        CategoryValue = $('#VehicleTypeCategoryValue').val();
    }
    else if (CategoryType == '9') {
        CategoryValue = $('#IDProofCategoryValue').val();
    }
    else if (CategoryType == '10') {
        CategoryValue = $('#DesignationCategoryValue').val();
    }
    else if (CategoryType == '11') {
        CategoryValue = $('#MaterialCategoryValue').val();
    }
    else if (CategoryType == '27') {
        CategoryValue = $('#LeaveTypeCategoryValue').val();
        CategoryDetailValue = $('#CategoryDetailValue').val();
        CategoryDetailValue1 = $('#CategoryDetailValue1').val();
        
    }
    else if (CategoryType == '28') {
        CategoryValue = $('#ParkingTypeCategoryValue').val();
    }
    else if (CategoryType == '29') {
        CategoryValue = $('#BayTypeCategoryValue').val();
    }
    else if (CategoryType == '30') {
        CategoryValue = $('#RoasterScheduleCategoryValue').val();
    }
    $.ajax({
        cache: false,
        type: "POST",
        async: false,
        url: ITX3ResolveUrl('Category/QuickAddCategoryFormData'),
        data: JSON.stringify({ CategoryType: CategoryType, CategoryValue: CategoryValue, SiteID: SiteId, CategoryDetailValue: CategoryDetailValue, CategoryDetailValue1: CategoryDetailValue1 }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            debugger;
            if (data.Result == 'Message') {
                if (data.Message == 'RedirectToLoginPage') {
                    ev.preventDefault();
                    window.location = ITX3ResolveUrl("Login/Index");
                }
                $('#MessageCategoryForm').text(data.Message);
                $('#MessageCategoryForm').css('display', 'block');
                $('#ValidationForVisitorTypeCategoryValue').html(data.Message);
                $('#ValidationForVisitorTypeCategoryValue').css('display', 'block');

                if (CategoryType == '1') {
                    $('#ValidationForVisitorTypeCategoryValue').html(data.Message);
                }
                else if (CategoryType == '2') {
                    $('#ValidationForCityCategoryValue').html(data.Message);
                }
                else if (CategoryType == '3') {
                    $('#ValidationForStateCategoryValue').html(data.Message);
                }
                else if (CategoryType == '4') {
                    $('#ValidationForCountryCategoryValue').html(data.Message);
                }
                else if (CategoryType == '5') {
                    $('#ValidationForMeetingLocationCategoryValue').html(data.Message);
                }
                else if (CategoryType == '6') {
                    $('#ValidationForDepartmentCategoryValue').html(data.Message);
                }
                else if (CategoryType == '7') {
                    $('#ValidationForVisitTypeCategoryValue').html(data.Message);
                }
                else if (CategoryType == '8') {
                    $('#ValidationForVehicleTypeCategoryValue').html(data.Message);
                }
                else if (CategoryType == '9') {
                    $('#ValidationForIDProofCategoryValue').html(data.Message);
                }
                else if (CategoryType == '10') {
                    $('#ValidationForDesignationCategoryValue').html(data.Message);
                }
                else if (CategoryType == '11') {
                    $('#ValidationForMaterialCategoryValue').html(data.Message);
                }
                else if (CategoryType == '27') {
                    $('#ValidationForLeaveTypeCategoryValue').html(data.Message);
                }
                else if (CategoryType == '28') {
                    $('#ValidationForParkingTypeCategoryValue').html(data.Message);
                }
                else if (CategoryType == '29') {
                    $('#ValidationForBayTypeCategoryValue').html(data.Message);
                }
                else if (CategoryType == '30') {
                    $('#ValidationForRoasterScheduleCategoryValue').html(data.Message);
                }
                flag = false;
            }
            else {

                if (CategoryType == '1') {
                    GetVisitorTypes(CategoryValue);
                }
                if (CategoryType == '2') {
                    GetCities(CategoryValue);
                }
                else if (CategoryType == '3') {
                    GetStates(CategoryValue);
                }
                else if (CategoryType == '4') {
                    GetCountries(CategoryValue);
                }
                else if (CategoryType == '5') {
                    GetMeetingLocations(CategoryValue);
                }
                else if (CategoryType == '6') {
                    GetDepartments(CategoryValue);
                }
                else if (CategoryType == '7') {
                    GetVisitTypes(CategoryValue);
                }
                else if (CategoryType == '8') {
                    GetVehicleType(CategoryValue);
                }
                else if (CategoryType == '9') {
                    GetIdProof(CategoryValue);
                }
                else if (CategoryType == '10') {
                    GetDesignation(CategoryValue);
                }
                else if (CategoryType == '11') {
                    GetMaterial(CategoryValue);
                }
                else if (CategoryType == '27') {
                    GetLeaveType(CategoryValue);
                }
                else if (CategoryType == '28') {
                    GetParkingType(CategoryValue, "formQuickAdd");
                }
                else if (CategoryType == '29') {
                    GetBayType(CategoryValue);
                }
                else if (CategoryType == '30') {
                    GetRoasterSchedule(CategoryValue);
                }
                flag = true;
            }

        },
        error: function (xhr, ret, e) {
            return false;
        }
    });
    return flag;
}


//Chandni M  : Regarding PopUI Changes : 11/07/2014
function QuickAddCategoryForAuthorization(CategoryType) {
    debugger;
    $('#MessageCategoryForm').css('display', 'none')
    $('#MessageCategoryForm').text('');
    var flag = true;
    var CategoryValue = '';
    var SiteId = '';
    if (CategoryType == '1') {
        CategoryValue = $('#VisitorTypeCategoryValue').val();
    }
    else if (CategoryType == '2') {
        CategoryValue = $('#CityCategoryValue').val();
    }
    else if (CategoryType == '3') {
        CategoryValue = $('#StateCategoryValue').val();
    }
    else if (CategoryType == '4') {
        CategoryValue = $('#CountryCategoryValue').val();
    }
    else if (CategoryType == '5') {
        CategoryValue = $('#MeetingLocationCategoryValue').val();
        SiteId = $('#site').val();
    }
    else if (CategoryType == '6') {
        CategoryValue = $('#DepartmentCategoryValue').val();
        SiteId = $('#SiteID').val();
    }
    else if (CategoryType == '7') {
        CategoryValue = $('#VisitTypeCategoryValue').val();
    }
    else if (CategoryType == '8') {
        CategoryValue = $('#VehicleTypeCategoryValue').val();
    }
    else if (CategoryType == '9') {
        CategoryValue = $('#IDProofCategoryValue').val();
    }
    else if (CategoryType == '10') {
        CategoryValue = $('#DesignationCategoryValue').val();
    }
    else if (CategoryType == '11') {
        CategoryValue = $('#MaterialCategoryValue').val();
    }
    else if (CategoryType == '27') {
        CategoryValue = $('#LeaveTypeCategoryValue').val();
    }
    $.ajax({
        cache: false,
        type: "POST",
        async: false,
        url: ITX3ResolveUrl('Category/QuickAddCategoryFormData'),
        data: JSON.stringify({ CategoryType: CategoryType, CategoryValue: CategoryValue, SiteID: SiteId }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            debugger;
            if (data.Result == 'Message') {
                if (data.Message == 'RedirectToLoginPage') {
                    ev.preventDefault();
                    window.location = ITX3ResolveUrl("Login/Index");
                }
                $('#MessageCategoryForm').text(data.Message);
                $('#MessageCategoryForm').css('display', 'block');
                $('#ValidationForVisitorTypeCategoryValue').html(data.Message);
                $('#ValidationForVisitorTypeCategoryValue').css('display', 'block');

                if (CategoryType == '1') {
                    $('#ValidationForVisitorTypeCategoryValue').html(data.Message);
                }
                else if (CategoryType == '2') {
                    $('#ValidationForCityCategoryValue').html(data.Message);
                }
                else if (CategoryType == '3') {
                    $('#ValidationForStateCategoryValue').html(data.Message);
                }
                else if (CategoryType == '4') {
                    $('#ValidationForCountryCategoryValue').html(data.Message);
                }
                else if (CategoryType == '5') {
                    $('#ValidationForMeetingLocationCategoryValue').html(data.Message);
                }
                else if (CategoryType == '6') {
                    $('#ValidationForDepartmentCategoryValue').html(data.Message);
                }
                else if (CategoryType == '7') {
                    $('#ValidationForVisitTypeCategoryValue').html(data.Message);
                }
                else if (CategoryType == '8') {
                    $('#ValidationForVehicleTypeCategoryValue').html(data.Message);
                }
                else if (CategoryType == '9') {
                    $('#ValidationForIDProofCategoryValue').html(data.Message);
                }
                else if (CategoryType == '10') {
                    $('#ValidationForDesignationCategoryValue').html(data.Message);
                }
                else if (CategoryType == '11') {
                    $('#ValidationForMaterialCategoryValue').html(data.Message);
                }
                else if (CategoryType == '27') {
                    $('#ValidationForLeaveTypeCategoryValue').html(data.Message);
                }

                flag = false;
            }
            else {

                if (CategoryType == '1') {
                    GetVisitorTypes(CategoryValue);
                }
                if (CategoryType == '2') {
                    GetCities(CategoryValue);
                }
                else if (CategoryType == '3') {
                    GetStates(CategoryValue);
                }
                else if (CategoryType == '4') {
                    GetCountries(CategoryValue);
                }
                else if (CategoryType == '5') {
                    SitewiseMeetingLocationForQuickAdd(CategoryValue);
                }
                else if (CategoryType == '6') {
                    GetDepartments(CategoryValue);
                }
                else if (CategoryType == '7') {
                    GetVisitTypes(CategoryValue);
                }
                else if (CategoryType == '8') {
                    GetVehicleType(CategoryValue);
                }
                else if (CategoryType == '9') {
                    GetIdProof(CategoryValue);
                }
                else if (CategoryType == '10') {
                    GetDesignation(CategoryValue);
                }
                else if (CategoryType == '11') {
                    GetMaterial(CategoryValue);
                }
                else if (CategoryType == '27') {
                    GetLeaveType(CategoryValue);
                }
                flag = true;
            }

        },
        error: function (xhr, ret, e) {
            return false;
        }
    });
    return flag;
}

//Chandni M  : Regarding PopUI Changes : 11/07/2014
function QuickAddVisitorCompany() {
    debugger;
    $('#MessageForm').css('display', 'none')
    $('#MessageForm').text('');

    var fVisitorCompanyName = $('#fVisitorCompanyName').val();
    var fAddress = $('#fAddress').val();
    var fPhone = $('#fPhone').val();
    var CityID = $('#VisitorCompanyCityID').val();

    var StateID = $('#VisitorCompanyStateID').val();
    var CountryID = $('#VisitorCompanyCountryID').val();
    var pincode = $('#fPincode').val();
    var UDFData = '';
    try {
        for (var i = 1; i <= $('#hdnCountUDF').val(); i++) {
            var UDFID = '.clsID-' + i;
            var LabelName = "#txtLabelName-" + i;
            if ($(LabelName).val() != "") {
                UDFData = UDFData + 'Œ' + $(LabelName).val() + 'æ' + $(UDFID).val();
            }
        }
    } catch (e) {

    }
    $.ajax({
        cache: false,
        type: "POST",
        async: false,
        url: ITX3ResolveUrl('VisitorCompany/QuickAddVisitorCompanyFormData'),
        data: JSON.stringify({ VCompanyName: fVisitorCompanyName, Address: fAddress, Phone: fPhone, cityId: CityID, Pincode: pincode, StateId: StateID, CountryId: CountryID, UDFData: UDFData }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.Result == 'Message') {
                globalDialogflag = false;
                $('#MessageForm').text(data.Message);
                $('#MessageForm').css('display', 'block');
            }
            else {
                GetVisitorCompanies(fVisitorCompanyName, "formQuickAdd");
                GetVisitorCompaniesPopUp();         //After Quick Add company , All Dropdown list should be reloaded in Quick Edit Visitor Company & Quick Add visitor
                LoadQuickEditForm();                //After Quick Add Company ,Company Quick Edit Pop Up should be reloaded with latest values.
                LoadVisitorQuickAddForm();          // If new Company Added by QuickAdd then Visitor Quick Add  PopUp should be reloaded                
                globalDialogflag = true;
            }
        },
        error: function (xhr, ret, e) {
        }
    });

}


//Chandni M  : Regarding PopUI Changes : 11/07/2014
function QuickAddVisitorCompanyForNew() {
    debugger;
    $('#MessageForm').css('display', 'none')
    $('#MessageForm').text('');

    var fVisitorCompanyName = $('#fVisitorCompanyName').val();
    var fAddress = $('#fAddress').val();
    var fPhone = $('#fPhone').val();
    var CityID = $('#VisitorCompanyCityID').val();
    var StateID = $('#VisitorCompanyStateID').val();
    var CountryID = $('#VisitorCompanyCountryID').val();
    var pincode = $('#fPincode').val();
    var fGSTNumber = $('#fGSTNumber').val();
    var UDFData = '';
    try {
        for (var i = 1; i <= $('#hdnCountUDF').val(); i++) {
            var UDFID = '.clsID-' + i;
            var LabelName = "#txtLabelName-" + i;
            if ($(LabelName).val() != "") {
                UDFData = UDFData + 'Œ' + $(LabelName).val() + 'æ' + $(UDFID).val();
            }
        }
    } catch (e) {

    }
    $.ajax({
        cache: false,
        type: "POST",
        async: false,
        url: ITX3ResolveUrl('VisitorCompany/QuickAddVisitorCompanyFormData'),
        data: JSON.stringify({ VCompanyName: fVisitorCompanyName, Address: fAddress, Phone: fPhone, cityId: CityID, Pincode: pincode, StateId: StateID, CountryId: CountryID, UDFData: UDFData, GSTNumber: fGSTNumber }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.Result == 'Message') {
                globalDialogflag = false;
                $('#MessageForm').text(data.Message);
                $('#MessageForm').css('display', 'block');
            }
            else {
                GetVisitorCompanies(fVisitorCompanyName, "formQuickAdd");
                //GetVisitorCompaniesPopUp();         //After Quick Add company , All Dropdown list should be reloaded in Quick Edit Visitor Company & Quick Add visitor
                //LoadQuickEditForm();                //After Quick Add Company ,Company Quick Edit Pop Up should be reloaded with latest values.
                LoadVisitorQuickAddForm();          // If new Company Added by QuickAdd then Visitor Quick Add  PopUp should be reloaded                
                globalDialogflag = true;
            }
        },
        error: function (xhr, ret, e) {
        }
    });

}



//Chandni M  : Regarding PopUI Changes : 11/07/2014
function QuickEditVisitorCompany() {

    debugger;

    $('#MessageEditForm').css('display', 'none')
    $('#MessageEditForm').text('');
    var VCompanyId = $('#feVisitorCompanyID').val();
    var fAddress = $('#feAddress').val();
    var fPhone = $('#fePhone').val();
    var CityID = $('#CityID').val();
    var Pincode = $('#fPincode').val();
    var StateID = $('#StateID').val();
    var CountryID = $('#CountryID').val();
    var fVisitorCompanyName = $('#VisitorCompanyName').val();
    var fGSTNumber = $('#feGSTNumber').val();
    var UDFData = '';
    try {
        for (var i = 1; i <= $('#EdithdnCountUDF').val(); i++) {
            var UDFID = '.EditclsID-' + i;
            var LabelName = "#EdittxtLabelName-" + i;
            if ($(LabelName).val() != "") {
                UDFData = UDFData + 'Œ' + $(LabelName).val() + 'æ' + $(UDFID).val();
            }
        }
    } catch (e) {

    }
    $.ajax({
        cache: false,
        type: "POST",
        async: false,
        url: ITX3ResolveUrl('VisitorCompany/QuickEditVisitorCompanyFormData'),
        data: JSON.stringify({ VCompanyId: VCompanyId, VCompanyName: fVisitorCompanyName, Address: fAddress, Phone: fPhone, cityId: CityID, Pincode: Pincode, StateId: StateID, CountryId: CountryID, UDFData: UDFData, GSTNumber: fGSTNumber }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            if (data.Result == 'Message') {

                $('#MessageEditForm').text(data.Message);
                $('#MessageEditForm').css('display', 'block');
            }
            else {

                GetVisitorCompanies(fVisitorCompanyName, "formQuickEdit");
                globalDialogflag = true;
            }

        },
        error: function (xhr, ret, e) {

        }
    });

}

//Chandni M  : Regarding PopUI Changes : 11/07/2014
function QuickAddVisitor() {
    debugger;
    $('#MessageVisitorForm').css('display', 'none')
    $('#MessageVisitorForm').text('');
    var fvVisitorName = $('#fvVisitorName').val();
    var fvVisitorCompanyID = $('#fvVisitorCompanyID').val();
    var fvVisitorPhone = $('#fvVisitorPhone').val();
    var fvDateOfBirthString = $('#DateOfBirthString').val();
    var fvVisitorEmail = $('#fvVisitorEmail').val();
    var fvVisitorTypeID = $('#fvVisitorTypeID').val();
    var isMobileNoVerified = $("#hdnisMobileNoVerified").val();
    var fvSafetyTrainingDateString = $('#fvSafetyTrainingDateString').val();
    var fvLicenseNumber = $('#fvLicenseNumber').val();
    var fvLicenseExpiryDateString = $('#fvLicenseExpiryDateString').val();

    if (isMobileNoVerified == undefined)
        isMobileNoVerified = 0;
    var UDFData = '';
    try {
        for (var i = 1; i <= $('#VhdnCountUDF').val(); i++) {
            var UDFID = '.VclsID-' + i;
            var LabelName = "#VtxtLabelName-" + i;
            if ($(LabelName).val() != "") {
                UDFData = UDFData + 'Œ' + $(LabelName).val() + 'æ' + $(UDFID).val();
            }
        }
    } catch (e) {

    }
    $.ajax({
        cache: false,
        type: "POST",
        async: false,
        url: ITX3ResolveUrl('Visitor/QuickAddVisitorFormData'),
        data: JSON.stringify({ VisitorName: fvVisitorName, VisitorCompanyId: fvVisitorCompanyID, VisitorPhone: fvVisitorPhone, DateOfBirth: fvDateOfBirthString,
            VisitorEmail: fvVisitorEmail, VisitorTypeId: fvVisitorTypeID, UDFData: UDFData, MobileNoVerified: isMobileNoVerified, SafetyTrainingDate: fvSafetyTrainingDateString
            , LicenseNumber: fvLicenseNumber, LicenseExpiryDateString: fvLicenseExpiryDateString
        }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            if (data.Result == 'Message') {
                if (data.Message == 'Redirect to Login Page') {
                    ev.preventDefault();
                    window.location = ITX3ResolveUrl("Login/Index");
                }
                globalDialogflag = false;
                $('#MessageVisitorForm').text(data.Message);
                $('#MessageVisitorForm').css('display', 'block');
                return false;
            }
            else {
                globalDialogflag = true;
                GetVisitorsByVisitorCompany(data.VisitorId);

                return true;
            }
        },
        error: function (xhr, ret, e) {
            return false;
        }
    });

}

function QuickAddVisitorofOutward() {
    debugger;
    $('#MessageVisitorForm').css('display', 'none')
    $('#MessageVisitorForm').text('');
    var fvVisitorName = $('#fvVisitorName').val();
    var fvVisitorCompanyID = $('#fvVisitorCompanyID').val();
    var fvVisitorPhone = $('#fvVisitorPhone').val();
    var fvDateOfBirthString = $('#DateOfBirthString').val();
    var fvVisitorEmail = $('#fvVisitorEmail').val();
    var fvVisitorTypeID = $('#VisitorTypeID').val();
    var fvSafetyTrainingDateString = $('#fvSafetyTrainingDateString').val();
    var fvLicenseNumber = $('#fvLicenseNumber').val();
    var fvLicenseExpiryDateString = $('#fvLicenseExpiryDateString').val();

    var UDFData = '';
    try {
        for (var i = 1; i <= $('#VhdnCountUDF').val(); i++) {
            var UDFID = '.VclsID-' + i;
            var LabelName = "#VtxtLabelName-" + i;
            if ($(LabelName).val() != "") {
                UDFData = UDFData + 'Œ' + $(LabelName).val() + 'æ' + $(UDFID).val();
            }
        }
    } catch (e) {

    }
    $.ajax({
        cache: false,
        type: "POST",
        async: false,
        url: ITX3ResolveUrl('Visitor/QuickAddVisitorFormData'),
        data: JSON.stringify({ VisitorName: fvVisitorName, VisitorCompanyId: fvVisitorCompanyID, VisitorPhone: fvVisitorPhone, DateOfBirth: fvDateOfBirthString, VisitorEmail: fvVisitorEmail, VisitorTypeId: fvVisitorTypeID, UDFData: UDFData, SafetyTrainingDate: fvSafetyTrainingDateString
        , LicenseNumber: fvLicenseNumber, LicenseExpiryDateString: fvLicenseExpiryDateString
        }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            if (data.Result == 'Message') {
                if (data.Message == 'Redirect to Login Page') {
                    ev.preventDefault();
                    window.location = ITX3ResolveUrl("Login/Index");
                }
                globalDialogflag = false;
                $('#MessageVisitorForm').text(data.Message);
                $('#MessageVisitorForm').css('display', 'block');
                return false;
            }
            else {
                globalDialogflag = true;
                GetAutoCompltetextBoxForQuickAddVisitor(fvVisitorCompanyID, fvVisitorName, data.VisitorId, fvVisitorPhone);

                return true;
            }
        },
        error: function (xhr, ret, e) {
            return false;
        }
    });

}

function QuickAddVisitorCompanyForRegistration() {

    $('#MessageForm').css('display', 'none')
    $('#MessageForm').text('');

    var fVisitorCompanyName = $('#fVisitorCompanyName').val();
    var fAddress = $('#fAddress').val();
    var fPhone = $('#fPhone').val();
    var CityID = $('#VisitorCompanyCityID').val();
    var StateID = $('#VisitorCompanyStateID').val();
    var CountryID = $('#VisitorCompanyCountryID').val();

    $.ajax({
        cache: false,
        type: "POST",
        async: false,
        url: ITX3ResolveUrl('VisitorCompany/QuickAddVisitorCompanyFormData'),
        data: JSON.stringify({ VCompanyName: fVisitorCompanyName, Address: fAddress, Phone: fPhone, cityId: CityID, StateId: StateID, CountryId: CountryID }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.Result == 'Message') {
                globalDialogflag = false;
                $('#MessageForm').text(data.Message);
                $('#MessageForm').css('display', 'block');
            }
            else {
                GetVisitorCompanies(fVisitorCompanyName, "formQuickAdd");
                GetVisitorCompaniesPopUp();         //After Quick Add company , All Dropdown list should be reloaded in Quick Edit Visitor Company & Quick Add visitor
                //                LoadQuickEditForm();                //After Quick Add Company ,Company Quick Edit Pop Up should be reloaded with latest values.
                //                LoadVisitorQuickAddForm();          // If new Company Added by QuickAdd then Visitor Quick Add  PopUp should be reloaded                
                globalDialogflag = true;
            }
        },
        error: function (xhr, ret, e) {
        }
    });

}

//Chandni M  : Regarding PopUI Changes : 11/07/2014
function QuickEditVisitor() {

    $('#MessageVisitorEditForm').css('display', 'none')
    $('#MessageVisitorEditForm').text('');
    var feVisitorName = $('#VisitorName').val();
    var feVisitorID = $('#feVisitorID').val();
    var feVisitorPhone = $('#feVisitorPhone').val();
    var feVisitorEmail = $('#feVisitorEmail').val();
    var feSafetyTrainingDateString = $('#feSafetyTrainingDateString').val();
    var feLicenseNumber = $('#feLicenseNumber').val();
    var feLicenseExpiryDateString = $('#feLicenseExpiryDateString').val();
    debugger;
    var fehdnisMobileNoVerified = $('#hdnisMobileNoVerified').val();
    if (fehdnisMobileNoVerified == undefined) {
        fehdnisMobileNoVerified = 0;
    }

    var UDFData = '';
    try {
        for (var i = 1; i <= $('#VEdithdnCountUDF').val(); i++) {
            var UDFID = '.VEditclsID-' + i;
            var LabelName = "#VEdittxtLabelName-" + i;
            if ($(LabelName).val() != "") {
                UDFData = UDFData + 'Œ' + $(LabelName).val() + 'æ' + $(UDFID).val();
            }
        }
    } catch (e) {

    }
    $.ajax({
        cache: false,
        type: "POST",
        async: false,
        url: ITX3ResolveUrl('Visitor/QuickEditVisitorFormData'),
        data: JSON.stringify({ VisitorName: feVisitorName, VisitorId: feVisitorID, VisitorPhone: feVisitorPhone, VisitorEmail: feVisitorEmail, UDFData: UDFData, MobileNoVerified: fehdnisMobileNoVerified, SafetyTrainingDate: feSafetyTrainingDateString
            , LicenseNumber: feLicenseNumber, LicenseExpiryDateString: feLicenseExpiryDateString
        }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            if (data.Result == 'Message') {
                if (data.Message == 'Redirect to Login Page') {
                    ev.preventDefault();
                    window.location = ITX3ResolveUrl("Login/Index");
                }

                $('#MessageVisitorEditForm').text(data.Message);
                $('#MessageVisitorEditForm').css('display', 'block');
                return false;
            }
            else {

                GetVisitorsByVisitorCompany(feVisitorID);
                globalDialogflag = true;
                return true;
            }
        },
        error: function (xhr, ret, e) {
            return false;
        }
    });

}

//Chandni M  : Regarding PopUI Changes : 11/07/2014
function QuickAddGatesFormData() {

    $('#MessageGateForm').css('display', 'none')
    $('#MessageGateForm').text('');
    var GateName = $('#fGateName').val();

    $.ajax({
        cache: false,
        type: "POST",
        async: false,
        url: ITX3ResolveUrl('Gates/QuickAddGatesFormData'),
        data: JSON.stringify({ GateName: GateName }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            if (data.Result == 'Message') {
                DialogFlag = false;
                if (data.Message == 'Redirect to Login Page') {
                    ev.preventDefault();
                    window.location = ITX3ResolveUrl("Login/Index");
                }
                else if (data.Message == 'Redirect to Home Page') {
                    ev.preventDefault();
                    window.location = ITX3ResolveUrl("Home/Index");
                }

                $('#MessageGateForm').text(data.Message);
                $('#MessageGateForm').css('display', 'block');
                return false;
            }
            else {
                DialogFlag = true;
                GetGates(data.GateID, GateName);
                return true;
            }
        },
        error: function (xhr, ret, e) {
            return false;
        }
    });

}

//Chandni M  : Regarding PopUI Changes : 11/07/2014
function QuickAddRolesFormData() {

    $('#MessageRoleForm').css('display', 'none')
    $('#MessageRoleForm').text('');

    $.ajax({
        cache: false,
        type: "POST",
        traditional: true,
        url: ITX3ResolveUrl('Role/QuickAddRolesFormData'),
        data: GetRoleModel(),
        dataType: "json",
        success: function (data) {

            if (data.Result == 'Message') {
                DialogFlag = false;
                if (data.Message == 'Redirect to Login Page') {
                    window.location = ITX3ResolveUrl("Login/Index");
                }

                $('#MessageRoleForm').text(data.Message);
                $('#MessageRoleForm').css('display', 'block');
                return false;
            }
            else {
                DialogFlag = true;
                GetRoles(data.RoleID);
                return true;
            }
        },
        error: function (xhr, ret, e) {
            return false;
        }
    });

}
function GetRoleModel() {
    DialogFlag = false;
    var Role = {};
    Role.RoleName = $("#fRoleName").val();
    Role.Description = $("#fDescription").val();
    Role.WriteAppointments = $("#DDForWriteAppointments").val();
    if ($("#GateSignificance").val() != null && $("#GateSignificance").val() != '') {
        Role.GateSignificance = $("#fGateSignificance").val();
    }
    if ($("#fAccessAllSites").val() != null && $("#fAccessAllSites").val() != '') {
        Role.AccessAllSites = $("#fAccessAllSites").val();
    }
    Role.ReadAppointments = $("#fDashboardAppointment").val();
    Role.DashboardDepartmentWiseDistribution = $("#fDashboardDepartmentWiseDistribution").val();
    Role.DashboardHostWiseDistribution = $("#fDashboardHostWiseDistribution").val();
    Role.DashboardWalkInVisitors = $("#fDashboardWalkInVisitors").val();
    Role.DashboardVisitorStatus = $("#fDashboardVisitorStatus").val();
    Role.lstFormRole = GetFormRolesModel();
    return Role;
}

function GetFormRolesModel() {
    var lstFormRole = [];
    var FormRole = {};
    var strCount = '#ObjectRole_lstFormRole_Count';
    var count = parseInt($(strCount).val());
    for (var i = 0; i < count; i++) {
        var FormRole = {};
        var strFormID = '#ObjectRole_lstFormRole_' + i.toString() + '__FormID';
        var strFormRoleID = '#ObjectRole_lstFormRole_' + i.toString() + '__FormRoleID';
        var strRoleID = '#ObjectRole_lstFormRole_' + i.toString() + '__RoleID';
        var strFormName = '#ObjectRole_lstFormRole_' + i.toString() + '__FormName';
        var strMenu = '#ObjectRole_lstFormRole_' + i.toString() + '__Menu';
        var id1 = "[" + i + "].AllowRead";
        var id2 = "[" + i + "].AllowUpdate";
        var id3 = "[" + i + "].AllowInsert";
        var id4 = "[" + i + "].AllowDelete";
        chkBox1 = document.getElementById(id1);
        chkBox2 = document.getElementById(id2);
        chkBox3 = document.getElementById(id3);
        chkBox4 = document.getElementById(id4);
        if (chkBox1 != null) FormRole.AllowRead = chkBox1.checked;
        if (chkBox2 != null) FormRole.AllowUpdate = chkBox2.checked;
        if (chkBox3 != null) FormRole.AllowInsert = chkBox3.checked;
        if (chkBox4 != null) FormRole.AllowDelete = chkBox4.checked;
        FormRole.FormID = $(strFormID).val();
        FormRole.FormRoleID = $(strFormRoleID).val();
        FormRole.RoleID = $(strRoleID).val();
        FormRole.FormName = $(strFormName).val();
        FormRole.Menu = $(strMenu).val();
        lstFormRole[i] = FormRole;
    }

    return lstFormRole;
}


function CheckPhoneNo(Fieldname, Fieldvalue) {

    $('#MessageForm').css('display', 'none')
    $('#MessageForm').text('');

    if (Fieldvalue.length > 0) {
        $.ajax({
            cache: false,
            type: "POST",
            async: false,
            url: ITX3ResolveUrl('VisitorGatepass/CheckForEmailAndPhone'),
            data: JSON.stringify({ Fieldname: Fieldname, Fieldvalue: Fieldvalue }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.Result == 'Message') {
                    $('#MessageForm').text(data.Message);
                    $('#MessageForm').css('display', 'block');
                }
            },
            error: function (xhr, ret, e) {

            }
        });
    }
}


function CheckEmailPhoneNo(Fieldname, Fieldvalue, EditVisitor) {

    if (EditVisitor == "EditVisitor") {
        $('#MessageVisitorEditForm').css('display', 'none')
        $('#MessageVisitorEditForm').text('');
    }
    else {
        $('#MessageVisitorForm').css('display', 'none')
        $('#MessageVisitorForm').text('');
    }


    if (Fieldvalue.length > 0) {
        $.ajax({
            cache: false,
            type: "POST",
            async: false,
            url: ITX3ResolveUrl('VisitorGatepass/CheckForEmailAndPhone'),
            data: JSON.stringify({ Fieldname: Fieldname, Fieldvalue: Fieldvalue }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.Result == 'Message') {

                    if (EditVisitor == "EditVisitor") {
                        $('#MessageVisitorEditForm').text(data.Message);
                        $('#MessageVisitorEditForm').css('display', 'block');
                    }
                    else {
                        $('#MessageVisitorForm').text(data.Message);
                        $('#MessageVisitorForm').css('display', 'block');
                    }

                }
            },
            error: function (xhr, ret, e) {

            }
        });
    }
}


function LoadQuickEditForm() {
    debugger;
    var ID = GetVisitorCompanyID();
    var lblname = GetUDF();
    $('#feVisitorCompanyID').val(ID);
    $.ajax({
        cache: false,
        type: "POST",
        async: false,
        url: ITX3ResolveUrl('VisitorCompany/GetVisitorCompanyInformation'),
        data: JSON.stringify({ VisitorCompanyID: ID }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.VC.CityID != 0)
                $('#CityID').val(data.VC.CityID);
            else
                $('#CityID').val("");

            if (data.VC.StateID != 0)
                $('#StateID').val(data.VC.StateID);
            else
                $('#StateID').val("");

            if (data.VC.CountryID != 0)
                $('#CountryID').val(data.VC.CountryID);
            else
                $('#CountryID').val("");

            $('#feAddress').val(data.VC.Address);
            $('#fePhone').val(data.VC.Phone);
            $('#fePincode').val(data.VC.Pincode);
            $('#VisitorCompanyName').val(data.VC.VisitorCompanyName);
            $('#feGSTNumber').val(data.VC.GSTNumber);
            // Add to show User Defined Fields with Value
            var UDFHtml = '';
            try {
                $('#divEditUDFData').html('');
                if (data.VC.lstVisitorCompanyUDFValue != null) {
                    var ids = 0;
                    for (var item = 0; item < data.VC.lstVisitorCompanyUDFValue.length; item++) {
                        if (data.VC.lstVisitorCompanyUDFValue[item].UserDefinedFieldsName != null) {
                            if (ids == 0) {
                                UDFHtml = UDFHtml + '<div class="col-md-12 form-group">' +
                                        '<label for="name" class="col-md-8">' + lblname + ' </label>' +
                                    '</div>' +
                                    '<hr style="margin-top: 0px; margin-bottom: 0px;" />';
                            }
                            ids++;
                            UDFHtml = UDFHtml + ' <div class="col-md-6"><div class="form-group"> ' +
                                '<label  class="col-md-8">' + data.VC.lstVisitorCompanyUDFValue[item].UserDefinedFieldsName + '</label>' +
                                '<div class="col-md-10">' +
                                    '<input type="text" id="EdittxtLabelName-' + ids + '" name="EdittxtLabelName" value="' + data.VC.lstVisitorCompanyUDFValue[item].UserDefinedFieldsValue + '" class="form-control EditclstxtLabelName" />' +
                                    '<span id="EditValidationFor-' + ids + '" class="field-validation-error" style="display: none">' +
                                    '</span>' +
                                    ' <input class="EditclsLabelName-' + ids + '"  type="hidden" value="' + data.VC.lstVisitorCompanyUDFValue[item].UserDefinedFieldsName + '" />' +
                                    '<input class="EditclsUseasMasterDetail-' + ids + '"  type="hidden" value="' + Number(data.VC.lstVisitorCompanyUDFValue[item].UseasMaster) + '" />' +
                                    ' <input class="EditclsIsMandatory-' + ids + '" type="hidden"  value="' + Number(data.VC.lstVisitorCompanyUDFValue[item].IsMandatory) + '" />' +
                                    '<input class="EditclsID-' + ids + '" type="hidden" name="EditUDFID"  value="' + data.VC.lstVisitorCompanyUDFValue[item].UserDefinedFieldsID + '" />' +
                                    '</div>' +
                                    '<div class="col-md-2">';
                            if (data.VC.lstVisitorCompanyUDFValue[item].IsMandatory) {
                                UDFHtml = UDFHtml + ' <i class="icon-asterisk required" style="margin-left: -25px;"></i>';
                            }
                            UDFHtml = UDFHtml + '</div>' +
                                    ' </div></div>';
                        }
                    }
                    UDFHtml = UDFHtml + ' <input type="hidden" id="EdithdnCountUDF" value="' + ids + '" />';
                    $('#divEditUDFData').append('<fieldset>' + UDFHtml + '</fieldset>');
                }
            } catch (e) {

            }
            GetAutoCompltetextBoxForMoreFields();
        },
        error: function (xhr, ret, e) {
        }

    });
}

function LoadVisitorQuickEditForm() {
    $('#feVisitorID').val($('#VisitorID').val());
    $('#feVisitorPhone').val($('#hdnvisitorphone').val());
    $('#feVisitorEmail').val($('#hdnvisitoremail').val());
    $('#feSafetyTrainingDateString').val($('#hdnvisitorsafetytrainingdate').val());
    $('#feLicenseNumber').val($('#hdnvisitorlicensenumber').val());
    $('#feLicenseExpiryDateString').val($('#hdnvisitorlicenseexpirydate').val());
}

function LoadVisitorQuickAddForm() {
    var i = GetVisitorCompanyID();
    $('#fvVisitorCompanyID').val(i);
    $("#fvVisitorCompanyID").attr('disabled', 'disabled');
}

function GetCapturedPhoto() {
    $.ajax({
        cache: false,
        type: "POST",
        async: false,
        url: ITX3ResolveUrl('Visitorgatepass/GetCapturedImage'),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data != '') {
                document.getElementById('imgVisitorPhoto').src = data.PhotoURL;
                //document.getElementById('imgVisitorPhotoZoom').src = data;
                $('#imgVisitorPhoto').show();
                //$('#imgVisitorPhotoZoom').show();
                //$('#divVisitorPhoto').show();
            }
            else {
                //$('#divVisitorPhoto').hide();

            }
        },
        error: function (xhr, ret, e) {
        }

    });

}
$("#RemoveImagefvforlong").click(function () {

    $('#preview-iframe').contents().find('img').attr('src', '');
    var VisitorID = $('#VisitorID').val();
    if (VisitorID > 0) {

        $.ajax({
            cache: false,
            type: "POST",
            async: true,
            url: ITX3ResolveUrl('Visitor/RemoveVisitorImage'),
            data: JSON.stringify({ visitorID: VisitorID }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.Result == 'OK') {
                    document.getElementById('FluVisitorPhoto1').value = '';
                    document.getElementById('imgVisitorPhotofv').src = '';
                    $('#divVisitorPhoto').hide();
                }
                else {
                    $('<div title="Information"></div>').dialog({
                        open: function (event, ui) {
                            $(this).html(data.Message);
                        }
                    });
                }
            },
            error: function (xhr, ret, e) {
            }

        });

    }
    else {
        document.getElementById('FluVisitorPhoto1').value = '';
        document.getElementById('imgVisitorPhotofv').src = '';
        $('#divVisitorPhotofv').hide();
    }
});

$("#RemoveImagefvforApp").click(function () {
    debugger;
    $('#preview-iframe').contents().find('img').attr('src', '');

    var VisitorID = $('#VisitorID').val();

    if (VisitorID > 0) {

        $.ajax({
            cache: false,
            type: "POST",
            async: true,
            url: ITX3ResolveUrl('Visitor/RemoveVisitorImage'),
            data: JSON.stringify({ visitorID: VisitorID }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.Result == 'OK') {
                    document.getElementById('FluAppointmentPhoto').value = '';
                    document.getElementById('imgVisitorPhoto').src = '';
                    document.getElementById('imgVisitorPhotoZoom').src = '';
                    $('#divVisitorPhoto').hide();
                }
                else {
                    $('<div title="Information"></div>').dialog({
                        open: function (event, ui) {
                            $(this).html(data.Message);
                        }
                    });
                }
            },
            error: function (xhr, ret, e) {
            }

        });

    }
    else {
        document.getElementById('FluAppointmentPhoto').value = '';
        document.getElementById('imgVisitorPhoto').src = '';
        document.getElementById('imgVisitorPhotoZoom').src = '';
        $('#divVisitorPhoto').hide();
    }
});

//Chandni M  : Regarding PopUI Changes : 11/07/2014
function QuickAddVehicleForNew() {
    debugger;
    $('#VehicleMasterError').css('display', 'none')
    $('#VehicleMasterError').html('');
    var fCVehicleTypeID = $('#fCVehicleTypeID').val();
    var fCLPNo = $('#fCLPNo').val();
    var fCOwnedBy = $('#fCOwnedBy').val();
    var fCDepartmentID = $('#fCDepartmentID').val();
    var fCUserID = $('#fCUserID').val();
    var fCVisitorCompanyID = $('#CreateVisitorCompanyID').val();
    var fCVisitorID = $('#CreateVisitorID').val();
    var fCContractorID = $('#CreateContractorID').val();
    var fCWorkerID = $('#CreateWorkerID').val();
    var fCurrentKms = $('#fCurrentKms').val();
    $.ajax({
        cache: false,
        type: "POST",
        async: false,
        url: ITX3ResolveUrl('Vehicle/QuickAddVehicleFormData'),
        data: JSON.stringify({ VehicleTypeID: fCVehicleTypeID, LPNo: fCLPNo, OwnedBy: fCOwnedBy, DepartmentID: fCDepartmentID, UserID: fCUserID
                                , VisitorCompanyID: fCVisitorCompanyID, VisitorID: fCVisitorID, ContractorID: fCContractorID, WorkerID: fCWorkerID, CurrentKms: fCurrentKms
        }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            if (data.Result == 'Message') {
                if (data.Message == 'Redirect to Login Page') {
                    ev.preventDefault();
                    window.location = ITX3ResolveUrl("Login/Index");
                }
                globalDialogflag = false;
                $('#VehicleMasterError').html(data.Message);
                $('#VehicleMasterError').css('display', 'block');
                return false;
            }
            else {
                globalDialogflag = true;
                $('#VehicleType').val(fCVehicleTypeID);
                ChangeVehicleType(data.VehicleRegistrationID);
                return true;
            }
        },
        error: function (xhr, ret, e) {
            return false;
        }
    });

}

function QuickAddParkingLocation() {
    debugger;
    var fvParkingLocationName = $('#fvParkingLocationName').val();
    var fvParkingTypeID = $('#fvParkingTypeID').val();
    $.ajax({
        cache: false,
        type: "POST",
        async: false,
        url: ITX3ResolveUrl('ParkingLocation/QuickAddParkingLocationFormData'),
        data: JSON.stringify({ ParkingLocationName: fvParkingLocationName, ParkingTypeID: fvParkingTypeID }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            debugger;
            if (data.Result == 'Message') {
                if (data.Message == 'Redirect to Login Page') {
                    ev.preventDefault();
                    window.location = ITX3ResolveUrl("Login/Index");
                }
                globalDialogflag = false;
                $('#MessageparkingForm').html(data.Message);
                $('#MessageparkingForm').css('display', 'block');
                return false;
            }
            else {
                globalDialogflag = true;
                $('#ParkingTypeID').val(fvParkingTypeID);
                GetParkingLocation(data.ParkingLocationID);
                return true;
            }
        },
        error: function (xhr, ret, e) {
            return false;
        }
    });
}