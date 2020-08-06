/**
 * Bootstrap
 */
import "jquery";
import "popper.js";
import "bootstrap";
import "bootstrap/dist/css/bootstrap.css";

/**
 * Argon
 */
import "argon-design-system-free/assets/js/plugins/datetimepicker";
import "argon-design-system-free/assets/css/argon-design-system.css";

/**
 * Web fonts
 */
import "@openfonts/cairo_latin";

$(document).ready(function () {
    //  Activate the Tooltips
    $('[data-toggle="tooltip"], [rel="tooltip"]').tooltip();

    // Activate Popovers and set color for popovers
    $('[data-toggle="popover"]').each(function () {
        var colorClass = $(this).data('color');
        $(this).popover({
            template: '<div class="popover popover-' +
                colorClass +
                '" role="tooltip"><div class="arrow"></div><h3 class="popover-header"></h3><div class="popover-body"></div></div>'
        });
    });
});