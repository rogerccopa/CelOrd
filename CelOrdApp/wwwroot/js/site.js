// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function validateCompanyName(evt) {
    var regex = new RegExp("^[a-z0-9_\-]+$");

    if (regex.test(evt.key)) {
        return true;
    }

    evt.preventDefault();
    return false;
}
