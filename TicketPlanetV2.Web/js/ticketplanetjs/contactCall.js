
$(document).ready(function () {

    $(".btnSubmitComment").on('click', function (e) {
        e.preventDefault();

        if ($("#name").val() === "" || $('#comment').val() === "" ||
                    $('#email').val() === "") {

            alertify.error("One or Two Fields is Empty");
            return;
        }
        var data = JSON.stringify({
            name: $("#name").val(),
            comment: $("#comment").val(),
            email: $("#email").val(),
            
        });

        $.ajax({
            type: "POST",
            url: "/Home/Contact",
            data: data,
            dataType: 'json',
            success: ''
        });
    });
});