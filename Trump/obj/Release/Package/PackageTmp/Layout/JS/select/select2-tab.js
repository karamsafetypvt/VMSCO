﻿jQuery(document).ready(function (a) {
    var b = a(document.body),
        c = !1,
        d = !1;
    b.on("keydown", function (a) {
        var b = a.keyCode ? a.keyCode : a.which;
        16 == b && (c = !0)
    }), b.on("keyup", function (a) {
        var b = a.keyCode ? a.keyCode : a.which;
        16 == b && (c = !1)
    }), b.on("mousedown", function (b) {
        d = !1, 1 != a(b.target).is('[class*="select2"]') && (d = !0)
    }), b.on("select2:opening", function (b) {
        d = !1, a(b.target).attr("data-s2open", 1)
    }), b.on("select2:closing", function (b) {
        a(b.target).removeAttr("data-s2open")
    }), b.on("select2:close", function (b) {
        var e = a(b.target);
        e.removeAttr("data-s2open");
        var f = e.closest("form"),
            g = f.has("[data-s2open]").length;
        if (0 == g && 0 == d) {
            var h = f.find(":input:enabled:not([readonly], input:hidden, button:hidden, textarea:hidden)").not(function () {
                return a(this).parent().is(":hidden")
            }),
                i = null;
            if (a.each(h, function (b) {
                var d = a(this);
                if (d.attr("id") == e.attr("id")) return i = c ? h.eq(b - 1) : h.eq(b + 1), !1
            }), null !== i) {
                var j = i.siblings(".select2").length > 0;
                j ? i.select2("open") : i.focus()
            }
        }
    }), b.on("focus", ".select2", function (b) {
        var c = a(this).siblings("select");
        0 == c.is("[disabled]") && 0 == c.is("[data-s2open]") && a(this).has(".select2-selection--single").length > 0 && (c.attr("data-s2open", 1), c.select2("open"))
    })
});