function GetCities(SelectedValue) {
    $('#CityID').empty();
    $.ajax({
        cache: false,
        type: "POST",
        async: false,
        url: ITX3ResolveUrl('Category/PopulateCategoriesDropDownList'),
        data: JSON.stringify({ CategoryType: 2 }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $('#CityID').append($('<option/>').html("Select"));
            $(data).each(function () {
                $('#CityID').append(
                            $('<option/>', {
                                value: this.CategoryID
                            }).html(this.CategoryValue)
                        );
            });
                        $('#CityID').val(SelectedValue);
        },
        error: function (xhr, ret, e) {
        }

    });

}