$(function () {
    $(document).on("click", "button.confirm-behavior,a.confirm-behavior", function () {
        return confirm($(this).data("message"));
    });
});
