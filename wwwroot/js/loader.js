// A $( document ).ready() block.
$(document).ready(function () {
    $('.waitButton').click(function () {
        var b = this;
        Site.ShowLoader();
        setTimeout(function () { $(b).attr("disabled", "disabled"); }, 0);
        return true;
    });
});