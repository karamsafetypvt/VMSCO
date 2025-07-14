
function keypressHandler(e) {
    if (e.which == 13 || e.which == 9) {
        $(this).blur();
        $('#LoadRecordsButton').focus().click();
        e.preventDefault();
    }
}

$('Form').keypress(keypressHandler);


function NumericTextbox(event) {
    if (event.shiftKey) {
        event.preventDefault();
    }

    if (event.keyCode == 46 || event.keyCode == 8) {
    }
    else {
        if (event.keyCode < 95) {
            if (event.keyCode < 48 || event.keyCode > 57) {
                event.preventDefault();
            }
        }
        else {
            if (event.keyCode < 96 || event.keyCode > 105) {
                event.preventDefault();
            }
        }
    }
}