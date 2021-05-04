$("#authorizationForm").submit(function (e) {

    e.preventDefault(); // avoid to execute the actual submit of the form.

    var form = $(this);
    var url = form.attr('action');

    $("#login").removeClass("is-invalid")
    $("#pass").removeClass("is-invalid")
    
    $.ajax({
        type: "POST",
        url: url,
        data: form.serialize(), // serializes the form's elements.
        dataType: 'json',
        success: function (data) {
            var res = Number.parseInt(data.Id);
            if (res > 0) {
                window.location.href = "Index"
            }
            else {
                $("#login").addClass("is-invalid")
                $("#pass").addClass("is-invalid")
            }
            console.log(data.IsAdmin);
        }
    });


});