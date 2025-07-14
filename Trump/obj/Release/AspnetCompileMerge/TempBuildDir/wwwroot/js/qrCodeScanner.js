var scanner = new Instascan.Scanner({ video: document.getElementById('preview'), scanPeriod: 5, mirror: false });
var qrcode = new QRCode("qrcode");
$('#btnScanQR').click(function () {
    debugger
    $('#btnScanQR').hide();
    $('#divQRCodeScan').show();
    $('#divQRCodeImg').hide();
    $('#lblQRCodeTxt').text("");
    $('#btnScanQR').attr('disabled', true)
    $('#btnScanCancel').show();



    scanner.addListener('scan', function (content) {
        //$('#divQRCode').show();
        //$('#lblQRCodeTxt').text(content);
        //$('#divQRCodeScan').hide();
        //$('#btnScanQR').attr('disabled', false);
        //scanner.getTracks()[0].stop()
        //localstream.getTracks()[0].stop();
        Instascan.Camera.getCameras().then(function (cameras) {
            if (cameras.length > 0) {
                scanner.stop(cameras[0]);
                GetScanedQRCodeDetail(content);

            }
        })


    });
    Instascan.Camera.getCameras().then(function (cameras) {
        if (cameras.length > 0) {
            scanner.start(cameras[0]);


            $('[name="options"]').on('change', function () {
                if ($(this).val() == 1) {
                    if (cameras[0] != "") {
                        scanner.start(cameras[0]);
                    } else {
                        GetDefault();
                        alert('No Front camera found!');
                    }
                } else if ($(this).val() == 2) {
                    if (cameras[1] != "") {
                        scanner.start(cameras[1]);
                    } else {
                        GetDefault();
                        alert('No Back camera found!');
                    }
                }
            });
        } else {
            GetDefault();
            console.error('No cameras found.');
            alert('No cameras found.');
        }
    }).catch(function (e) {
        GetDefault();
        console.error(e);
        alert(e);
    });


})

$('#btnScanCancel').click(function () {
    Instascan.Camera.getCameras().then(function (cameras) {
        if (cameras.length > 0) {
            scanner.stop(cameras[0]);
            $('#divBeforeScaned').show();
            $('#divAfterScaned').hide();
            $(".span").html("");
            GetDefault();
        }
    })
})

$('#btnPay').click(function () {

    
    $.ajax({
        url: 'Home/Phonepay',
        type: 'POST',
        data: { Amount: 10 },
        success: function (response) {
        },
        error: function (data) {

        }
    });
    
    qrcode.makeCode("ABC12345678");
    
    $('#divGeneratedQrcode').show();
    $('#divAfterScaned').hide();
    $('#divBeforeScaned').hide();
    $('#divQRCodeScan').hide()
    $('#divQRCodeImg').hide();
    $('#divQRCode').hide();
    $('#btnScanCancel').hide();
    $('#btnScanQR').hide();
    $('#btnScanQR').attr('disabled', true);
    $('#btnPay').hide();
    $('#btnScanCancel').hide();
    $('#btnQRCodeSave').show();
    
})

$('#btnQRCodeSave').click(function () {
    location.reload();
})

function GetDefault() {
    $('#divQRCodeScan').hide()
    $('#divQRCodeImg').show();
    $('#divQRCode').hide();
    $('#btnScanCancel').hide();
    $('#btnScanQR').show();
    $('#btnScanQR').attr('disabled', false);
    $('#btnPay').hide();
    $('#btnScanCancel').hide();
    $('#btnQRCodeSave').hide();

    $('#divGeneratedQrcode').hide();
    $('#divAfterScaned').hide();
    $('#divBeforeScaned').show();


}

function GetScanedQRCodeDetail(code) {
    debugger
    alert(code);
    if (code != null && code != undefined) {
        $.ajax({
            url: 'Home/GetScanedQRCodeDetail',
            type: 'POST',
            data: { QRCode: code },
            success: function (response) {
                if (response.isSuccess && response.data != null) {
                    $(".span").html("");
                    $('#divBeforeScaned').hide();
                    $('#divAfterScaned').show();

                    $('#lblVehicleCategory').text(response.data.vehicleCategory);
                    $('#lblEntryTime').text(response.data.entryTime);
                    $('#lblTimeSpent').text(response.data.timeSpent);
                    $('#lblAmount').text(response.data.amount);

                    $('#btnPay').show();
                    $('#btnScanQR').hide();
                }
            },
            error: function (data) {

            }
        });
    }

}


 