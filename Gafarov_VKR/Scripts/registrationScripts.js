
var occupiedLogins = [];
$.getJSON({
    url: '/Home/GetOccupiedLogins',
    async: false
})
.done(function (data) {
    data.forEach((login) => {
        occupiedLogins.push(login);
    });
})
.fail(function (jqxhr, textStatus, error) {
    var err = textStatus + ', ' + error;
})

$("#registrationForm").submit(function (e) {
    e.preventDefault(); // avoid to execute the actual submit of the form.

    var form = $(this);
    var url = form.attr('action');

    $("#login").removeClass("is-invalid")
    $("#pass2").removeClass("is-invalid")
    $("#pass1").removeClass("is-invalid")
    if ($("#pass1").val() != $("#pass2").val()) {
        $("#pass2").addClass("is-invalid")
        $("#pass1").addClass("is-invalid")
    } else {
        $.ajax({
            type: "POST",
            url: url,
            data: form.serialize(), // serializes the form's elements.
            success: function (data) {
                var res = Number.parseInt(data);
                if (res == 1) {
                    window.location.href = "Index"
                }
                else {
                    $("#login").addClass("is-invalid")
                }
            }
        });
    }
});