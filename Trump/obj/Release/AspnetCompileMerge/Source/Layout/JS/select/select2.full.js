!function () {
    if (window.define) var e = window.define;
    if (window.require) var t = window.require;
    if (window.jQuery && jQuery.fn && jQuery.fn.select2 && jQuery.fn.select2.amd) var e = jQuery.fn.select2.amd.define,
        t = jQuery.fn.select2.amd.require;
    var n, t, e;
    !function (r) {
        function i(e, t) {
            return _.call(e, t)
        }

        function o(e, t) {
            var n, r, i, o, s, a, l, c, u, d, h, p = t && t.split("/"),
                f = y.map,
                g = f && f["*"] || {};
            if (e && "." === e.charAt(0))
                if (t) {
                    for (p = p.slice(0, p.length - 1), e = e.split("/"), s = e.length - 1, y.nodeIdCompat && b.test(e[s]) && (e[s] = e[s].replace(b, "")), e = p.concat(e), u = 0; u < e.length; u += 1)
                        if (h = e[u], "." === h) e.splice(u, 1), u -= 1;
                        else if (".." === h) {
                            if (1 === u && (".." === e[2] || ".." === e[0])) break;
                            u > 0 && (e.splice(u - 1, 2), u -= 2)
                        }
                    e = e.join("/")
                } else 0 === e.indexOf("./") && (e = e.substring(2));
            if ((p || g) && f) {
                for (n = e.split("/"), u = n.length; u > 0; u -= 1) {
                    if (r = n.slice(0, u).join("/"), p)
                        for (d = p.length; d > 0; d -= 1)
                            if (i = f[p.slice(0, d).join("/")], i && (i = i[r])) {
                                o = i, a = u;
                                break
                            }
                    if (o) break;
                    !l && g && g[r] && (l = g[r], c = u)
                } !o && l && (o = l, a = c), o && (n.splice(0, a, o), e = n.join("/"))
            }
            return e
        }

        function s(e, t) {
            return function () {
                return p.apply(r, $.call(arguments, 0).concat([e, t]))
            }
        }

        function a(e) {
            return function (t) {
                return o(t, e)
            }
        }

        function l(e) {
            return function (t) {
                m[e] = t
            }
        }

        function c(e) {
            if (i(v, e)) {
                var t = v[e];
                delete v[e], w[e] = !0, h.apply(r, t)
            }
            if (!i(m, e) && !i(w, e)) throw new Error("No " + e);
            return m[e]
        }

        function u(e) {
            var t, n = e ? e.indexOf("!") : -1;
            return n > -1 && (t = e.substring(0, n), e = e.substring(n + 1, e.length)), [t, e]
        }

        function d(e) {
            return function () {
                return y && y.config && y.config[e] || {}
            }
        }
        var h, p, f, g, m = {},
            v = {},
            y = {},
            w = {},
            _ = Object.prototype.hasOwnProperty,
            $ = [].slice,
            b = /\.js$/;
        f = function (e, t) {
            var n, r = u(e),
                i = r[0];
            return e = r[1], i && (i = o(i, t), n = c(i)), i ? e = n && n.normalize ? n.normalize(e, a(t)) : o(e, t) : (e = o(e, t), r = u(e), i = r[0], e = r[1], i && (n = c(i))), {
                f: i ? i + "!" + e : e,
                n: e,
                pr: i,
                p: n
            }
        }, g = {
            require: function (e) {
                return s(e)
            },
            exports: function (e) {
                var t = m[e];
                return "undefined" != typeof t ? t : m[e] = {}
            },
            module: function (e) {
                return {
                    id: e,
                    uri: "",
                    exports: m[e],
                    config: d(e)
                }
            }
        }, h = function (e, t, n, o) {
            var a, u, d, h, p, y, _ = [],
                $ = typeof n;
            if (o = o || e, "undefined" === $ || "function" === $) {
                for (t = !t.length && n.length ? ["require", "exports", "module"] : t, p = 0; p < t.length; p += 1)
                    if (h = f(t[p], o), u = h.f, "require" === u) _[p] = g.require(e);
                    else if ("exports" === u) _[p] = g.exports(e), y = !0;
                    else if ("module" === u) a = _[p] = g.module(e);
                    else if (i(m, u) || i(v, u) || i(w, u)) _[p] = c(u);
                    else {
                        if (!h.p) throw new Error(e + " missing " + u);
                        h.p.load(h.n, s(o, !0), l(u), {}), _[p] = m[u]
                    }
                d = n ? n.apply(m[e], _) : void 0, e && (a && a.exports !== r && a.exports !== m[e] ? m[e] = a.exports : d === r && y || (m[e] = d))
            } else e && (m[e] = n)
        }, n = t = p = function (e, t, n, i, o) {
            if ("string" == typeof e) return g[e] ? g[e](t) : c(f(e, t).f);
            if (!e.splice) {
                if (y = e, y.deps && p(y.deps, y.callback), !t) return;
                t.splice ? (e = t, t = n, n = null) : e = r
            }
            return t = t || function () { }, "function" == typeof n && (n = i, i = o), i ? h(r, e, t, n) : setTimeout(function () {
                h(r, e, t, n)
            }, 4), p
        }, p.config = function (e) {
            return p(e)
        }, n._defined = m, e = function (e, t, n) {
            t.splice || (n = t, t = []), i(m, e) || i(v, e) || (v[e] = [e, t, n])
        }, e.amd = {
            jQuery: !0
        }
    } (), e("almond", function () { }), e("jquery", [], function () {
        var e = jQuery || $;
        return null == e && console && console.error && console.error("Select2: An instance of jQuery or a jQuery-compatible library was not found. Make sure that you are including jQuery before Select2 on your web page."), e
    }), e("select2/utils", [], function () {
        function e(e) {
            var t = e.prototype,
                    n = [];
            for (var r in t) {
                var i = t[r];
                "function" == typeof i && "constructor" !== r && n.push(r)
            }
            return n
        }
        var t = {};
        t.Extend = function (e, t) {
            function n() {
                this.constructor = e
            }
            var r = {}.hasOwnProperty;
            for (var i in t) r.call(t, i) && (e[i] = t[i]);
            return n.prototype = t.prototype, e.prototype = new n, e.__super__ = t.prototype, e
        }, t.Decorate = function (t, n) {
            function r() {
                var e = Array.prototype.unshift,
                        r = n.prototype.constructor.length,
                        i = t.prototype.constructor;
                r > 0 && (e.call(arguments, t.prototype.constructor), i = n.prototype.constructor), i.apply(this, arguments)
            }

            function i() {
                this.constructor = r
            }
            var o = e(n),
                    s = e(t);
            n.displayName = t.displayName, r.prototype = new i;
            for (var a = 0; a < s.length; a++) {
                var l = s[a];
                r.prototype[l] = t.prototype[l]
            }
            for (var c = (function (e) {
                var t = function () { };
                e in r.prototype && (t = r.prototype[e]);
                var i = n.prototype[e];
                return function () {
                    var e = Array.prototype.unshift;
                    return e.call(arguments, t), i.apply(this, arguments)
                }
            }), u = 0; u < o.length; u++) {
                var d = o[u];
                r.prototype[d] = c(d)
            }
            return r
        };
        var n = function () {
            this.listeners = {}
        };
        return n.prototype.on = function (e, t) {
            this.listeners = this.listeners || {}, e in this.listeners ? this.listeners[e].push(t) : this.listeners[e] = [t]
        }, n.prototype.trigger = function (e) {
            var t = Array.prototype.slice;
            this.listeners = this.listeners || {}, e in this.listeners && this.invoke(this.listeners[e], t.call(arguments, 1)), "*" in this.listeners && this.invoke(this.listeners["*"], arguments)
        }, n.prototype.invoke = function (e, t) {
            for (var n = 0, r = e.length; r > n; n++) e[n].apply(this, t)
        }, t.Observable = n, t.generateChars = function (e) {
            for (var t = "", n = 0; e > n; n++) {
                var r = Math.floor(36 * Math.random());
                t += r.toString(36)
            }
            return t
        }, t.bind = function (e, t) {
            return function () {
                e.apply(t, arguments)
            }
        }, t._convertData = function (e) {
            for (var t in e) {
                var n = t.split("-"),
                        r = e;
                if (1 !== n.length) {
                    for (var i = 0; i < n.length; i++) {
                        var o = n[i];
                        o = o.substring(0, 1).toLowerCase() + o.substring(1), o in r || (r[o] = {}), i == n.length - 1 && (r[o] = e[t]), r = r[o]
                    }
                    delete e[t]
                }
            }
            return e
        }, t.hasScroll = function (e, t) {
            var n = $(t),
                    r = t.style.overflowX,
                    i = t.style.overflowY;
            return r !== i || "hidden" !== i && "visible" !== i ? "scroll" === r || "scroll" === i ? !0 : n.innerHeight() < t.scrollHeight || n.innerWidth() < t.scrollWidth : !1
        }, t
    }), e("select2/results", ["jquery", "./utils"], function (e, t) {
        function n(e, t, r) {
            this.$element = e, this.data = r, this.options = t, n.__super__.constructor.call(this)
        }
        return t.Extend(n, t.Observable), n.prototype.render = function () {
            var t = e('<ul class="select2-results__options" role="tree"></ul>');
            return this.options.get("multiple") && t.attr("aria-multiselectable", "true"), this.$results = t, t
        }, n.prototype.clear = function () {
            this.$results.empty()
        }, n.prototype.displayMessage = function (t) {
            this.clear(), this.hideLoading();
            var n = e('<li role="treeitem" class="select2-results__option"></li>'),
                    r = this.options.get("translations").get(t.message);
            n.text(r(t.args)), this.$results.append(n)
        }, n.prototype.append = function (e) {
            this.hideLoading();
            var t = [];
            if (null == e.results || 0 === e.results.length) return void (0 === this.$results.children().length && this.trigger("results:message", {
                message: "noResults"
            }));
            e.results = this.sort(e.results);
            for (var n = 0; n < e.results.length; n++) {
                var r = e.results[n],
                        i = this.option(r);
                t.push(i)
            }
            this.$results.append(t)
        }, n.prototype.position = function (e, t) {
            var n = t.find(".select2-results");
            n.append(e)
        }, n.prototype.sort = function (e) {
            var t = this.options.get("sorter");
            return t(e)
        }, n.prototype.setClasses = function () {
            var t = this;
            this.data.current(function (n) {
                var r = e.map(n, function (e) {
                    return e.id.toString()
                }),
                        i = t.$results.find(".select2-results__option[aria-selected]");
                i.each(function () {
                    var t = e(this),
                            n = e.data(this, "data");
                    null != n.id && r.indexOf(n.id.toString()) > -1 ? t.attr("aria-selected", "true") : t.attr("aria-selected", "false")
                });
                var o = i.filter("[aria-selected=true]");
                o.length > 0 ? o.first().trigger("mouseenter") : i.first().trigger("mouseenter")
            })
        }, n.prototype.showLoading = function (e) {
            this.hideLoading();
            var t = this.options.get("translations").get("searching"),
                    n = {
                        disabled: !0,
                        loading: !0,
                        text: t(e)
                    },
                    r = this.option(n);
            r.className += " loading-results", this.$results.prepend(r)
        }, n.prototype.hideLoading = function () {
            this.$results.find(".loading-results").remove()
        }, n.prototype.option = function (t) {
            var n = document.createElement("li");
            n.className = "select2-results__option";
            var r = {
                role: "treeitem",
                "aria-selected": "false"
            };
            t.disabled && (delete r["aria-selected"], r["aria-disabled"] = "true"), null == t.id && delete r["aria-selected"], null != t._resultId && (n.id = t._resultId), t.children && (r.role = "group", r["aria-label"] = t.text, delete r["aria-selected"]);
            for (var i in r) {
                var o = r[i];
                n.setAttribute(i, o)
            }
            if (t.children) {
                var s = e(n),
                        a = document.createElement("strong");
                a.className = "select2-results__group"; 
                {
                    e(a)
                }
                this.template(t, a);
                for (var l = [], c = 0; c < t.children.length; c++) {
                    var u = t.children[c],
                            d = this.option(u);
                    l.push(d)
                }
                var h = e("<ul></ul>", {
                    "class": "select2-results__options select2-results__options--nested"
                });
                h.append(l), s.append(a), s.append(h)
            } else this.template(t, n);
            return e.data(n, "data", t), n
        }, n.prototype.bind = function (t) {
            var n = this,
                    r = t.id + "-results";
            this.$results.attr("id", r), t.on("results:all", function (e) {
                n.clear(), n.append(e.data), t.isOpen() && n.setClasses()
            }), t.on("results:append", function (e) {
                n.append(e.data), t.isOpen() && n.setClasses()
            }), t.on("query", function (e) {
                n.showLoading(e)
            }), t.on("select", function () {
                t.isOpen() && n.setClasses()
            }), t.on("unselect", function () {
                t.isOpen() && n.setClasses()
            }), t.on("open", function () {
                n.$results.attr("aria-expanded", "true"), n.$results.attr("aria-hidden", "false"), n.setClasses(), n.ensureHighlightVisible()
            }), t.on("close", function () {
                n.$results.attr("aria-expanded", "false"), n.$results.attr("aria-hidden", "true"), n.$results.removeAttr("aria-activedescendant")
            }), t.on("results:select", function () {
                var e = n.getHighlightedResults();
                if (0 !== e.length) {
                    var t = e.data("data");
                    "true" == e.attr("aria-selected") ? n.options.get("multiple") ? n.trigger("unselect", {
                        data: t
                    }) : n.trigger("close") : n.trigger("select", {
                        data: t
                    })
                }
            }), t.on("results:previous", function () {
                var e = n.getHighlightedResults(),
                        t = n.$results.find("[aria-selected]"),
                        r = t.index(e);
                if (0 !== r) {
                    var i = r - 1;
                    0 === e.length && (i = 0);
                    var o = t.eq(i);
                    o.trigger("mouseenter");
                    var s = n.$results.offset().top,
                            a = o.offset().top,
                            l = n.$results.scrollTop() + (a - s);
                    0 === i ? n.$results.scrollTop(0) : 0 > a - s && n.$results.scrollTop(l)
                }
            }), t.on("results:next", function () {
                var e = n.getHighlightedResults(),
                        t = n.$results.find("[aria-selected]"),
                        r = t.index(e),
                        i = r + 1;
                if (!(i >= t.length)) {
                    var o = t.eq(i);
                    o.trigger("mouseenter");
                    var s = n.$results.offset().top + n.$results.outerHeight(!1),
                            a = o.offset().top + o.outerHeight(!1),
                            l = n.$results.scrollTop() + a - s;
                    0 === i ? n.$results.scrollTop(0) : a > s && n.$results.scrollTop(l)
                }
            }), t.on("results:focus", function (e) {
                e.element.addClass("select2-results__option--highlighted")
            }), t.on("results:message", function (e) {
                n.displayMessage(e)
            }), e.fn.mousewheel && this.$results.on("mousewheel", function (e) {
                var t = n.$results.scrollTop(),
                        r = n.$results.get(0).scrollHeight - n.$results.scrollTop() + e.deltaY,
                        i = e.deltaY > 0 && t - e.deltaY <= 0,
                        o = e.deltaY < 0 && r <= n.$results.height();
                i ? (n.$results.scrollTop(0), e.preventDefault(), e.stopPropagation()) : o && (n.$results.scrollTop(n.$results.get(0).scrollHeight - n.$results.height()), e.preventDefault(), e.stopPropagation())
            }), this.$results.on("mouseup", ".select2-results__option[aria-selected]", function (t) {
                var r = e(this),
                        i = r.data("data");
                return "true" === r.attr("aria-selected") ? void (n.options.get("multiple") ? n.trigger("unselect", {
                    originalEvent: t,
                    data: i
                }) : n.trigger("close")) : void n.trigger("select", {
                    originalEvent: t,
                    data: i
                })
            }), this.$results.on("mouseenter", ".select2-results__option[aria-selected]", function () {
                var t = e(this).data("data");
                n.getHighlightedResults().removeClass("select2-results__option--highlighted"), n.trigger("results:focus", {
                    data: t,
                    element: e(this)
                })
            })
        }, n.prototype.getHighlightedResults = function () {
            var e = this.$results.find(".select2-results__option--highlighted");
            return e
        }, n.prototype.destroy = function () {
            this.$results.remove()
        }, n.prototype.ensureHighlightVisible = function () {
            var e = this.getHighlightedResults();
            if (0 !== e.length) {
                var t = this.$results.find("[aria-selected]"),
                        n = t.index(e),
                        r = this.$results.offset().top,
                        i = e.offset().top,
                        o = this.$results.scrollTop() + (i - r),
                        s = i - r;
                o -= 2 * e.outerHeight(!1), 2 >= n ? this.$results.scrollTop(0) : (s > this.$results.outerHeight() || 0 > s) && this.$results.scrollTop(o)
            }
        }, n.prototype.template = function (e, t) {
            var n = this.options.get("templateResult"),
                    r = n(e);
            null == r ? t.style.display = "none" : t.innerHTML = r
        }, n
    }), e("select2/keys", [], function () {
        var e = {
            BACKSPACE: 8,
            TAB: 9,
            ENTER: 13,
            SHIFT: 16,
            CTRL: 17,
            ALT: 18,
            ESC: 27,
            SPACE: 32,
            PAGE_UP: 33,
            PAGE_DOWN: 34,
            END: 35,
            HOME: 36,
            LEFT: 37,
            UP: 38,
            RIGHT: 39,
            DOWN: 40,
            DELETE: 46,
            isArrow: function (e) {
                switch (e = e.which ? e.which : e) {
                    case KEY.LEFT:
                    case KEY.RIGHT:
                    case KEY.UP:
                    case KEY.DOWN:
                        return !0
                }
                return !1
            }
        };
        return e
    }), e("select2/selection/base", ["jquery", "../utils", "../keys"], function (e, t, n) {
        function r(e, t) {
            this.$element = e, this.options = t, r.__super__.constructor.call(this)
        }
        return t.Extend(r, t.Observable), r.prototype.render = function () {
            var t = e('<span class="select2-selection"  role="combobox" aria-autocomplete="list" aria-haspopup="true" aria-expanded="false"></span>');
            return t.attr("title", this.$element.attr("title")), this.$selection = t, t
        }, r.prototype.bind = function (e) {
            var t = this,
                    r = (e.id + "-container", e.id + "-results");
            this.container = e, this.$selection.attr("aria-owns", r), this.$selection.on("keydown", function (e) {
                t.trigger("keypress", e), e.which === n.SPACE && e.preventDefault()
            }), e.on("results:focus", function (e) {
                t.$selection.attr("aria-activedescendant", e.data._resultId)
            }), e.on("selection:update", function (e) {
                t.update(e.data)
            }), e.on("open", function () {
                t.$selection.attr("aria-expanded", "true"), t._attachCloseHandler(e)
            }), e.on("close", function () {
                t.$selection.attr("aria-expanded", "false"), t.$selection.removeAttr("aria-activedescendant"), t.$selection.focus(), t._detachCloseHandler(e)
            }), e.on("enable", function () {
               // t.$selection.attr("tabindex", "0")
            }), e.on("disable", function () {
                //t.$selection.attr("tabindex", "-1")
            })
        }, r.prototype._attachCloseHandler = function (t) {
            e(document.body).on("mousedown.select2." + t.id, function (t) {
                var n = e(t.target),
                        r = n.closest(".select2"),
                        i = e(".select2.select2-container--open");
                i.each(function () {
                    var t = e(this);
                    if (this != r[0]) {
                        var n = t.data("element");
                        n.select2("close")
                    }
                })
            })
        }, r.prototype._detachCloseHandler = function (t) {
            e(document.body).off("mousedown.select2." + t.id)
        }, r.prototype.position = function (e, t) {
            var n = t.find(".selection");
            n.append(e)
        }, r.prototype.destroy = function () {
            this._detachCloseHandler(this.container)
        }, r.prototype.update = function () {
            throw new Error("The `update` method must be defined in child classes.")
        }, r
    }), e("select2/selection/single", ["jquery", "./base", "../utils", "../keys"], function (e, t, n) {
        function r() {
            r.__super__.constructor.apply(this, arguments)
        }
        return n.Extend(r, t), r.prototype.render = function () {
            var e = r.__super__.render.call(this);
            return e.addClass("select2-selection--single"), e.html('<span class="select2-selection__rendered"></span><span class="select2-selection__arrow" role="presentation"><b role="presentation"></b></span>'), e
        }, r.prototype.bind = function (e) {
            var t = this;
            r.__super__.bind.apply(this, arguments);
            var n = e.id + "-container";
            this.$selection.find(".select2-selection__rendered").attr("id", n), this.$selection.attr("aria-labelledby", n), this.$selection.on("mousedown", function (e) {
                1 === e.which && t.trigger("toggle", {
                    originalEvent: e
                })
            }), this.$selection.on("focus", function () { }), this.$selection.on("blur", function () { }), e.on("selection:update", function (e) {
                t.update(e.data)
            })
        }, r.prototype.clear = function () {
            this.$selection.find(".select2-selection__rendered").empty()
        }, r.prototype.display = function (e) {
            var t = this.options.get("templateSelection");
            return t(e)
        }, r.prototype.selectionContainer = function () {
            return e("<span></span>")
        }, r.prototype.update = function (e) {
            if (0 === e.length) return void this.clear();
            var t = e[0],
                    n = this.display(t);
            this.$selection.find(".select2-selection__rendered").html(n)
        }, r
    }), e("select2/selection/multiple", ["jquery", "./base", "../utils"], function (e, t, n) {
        function r() {
            r.__super__.constructor.apply(this, arguments)
        }
        return n.Extend(r, t), r.prototype.render = function () {
            var e = r.__super__.render.call(this);
            return e.addClass("select2-selection--multiple"), e.html('<ul class="select2-selection__rendered"></ul>'), e
        }, r.prototype.bind = function () {
            var t = this;
            r.__super__.bind.apply(this, arguments), this.$selection.on("click", function (e) {
                t.trigger("toggle", {
                    originalEvent: e
                })
            }), this.$selection.on("click", ".select2-selection__choice__remove", function (n) {
                var r = e(this),
                        i = r.parent(),
                        o = i.data("data");
                t.trigger("unselect", {
                    originalEvent: n,
                    data: o
                })
            })
        }, r.prototype.clear = function () {
            this.$selection.find(".select2-selection__rendered").empty()
        }, r.prototype.display = function (e) {
            var t = this.options.get("templateSelection");
            return t(e)
        }, r.prototype.selectionContainer = function () {
            var t = e('<li class="select2-selection__choice"><span class="select2-selection__choice__remove" role="presentation">&times;</span></li>');
            return t
        }, r.prototype.update = function (e) {
            if (this.clear(), 0 !== e.length) {
                for (var t = [], n = 0; n < e.length; n++) {
                    var r = e[n],
                            i = this.display(r),
                            o = this.selectionContainer();
                    o.append(i), o.data("data", r), t.push(o)
                }
                this.$selection.find(".select2-selection__rendered").append(t)
            }
        }, r
    }), e("select2/selection/placeholder", ["../utils"], function () {
        function e(e, t, n) {
            this.placeholder = this.normalizePlaceholder(n.get("placeholder")), e.call(this, t, n)
        }
        return e.prototype.normalizePlaceholder = function (e, t) {
            return "string" == typeof t && (t = {
                id: "",
                text: t
            }), t
        }, e.prototype.createPlaceholder = function (e, t) {
            var n = this.selectionContainer();
            return n.html(this.display(t)), n.addClass("select2-selection__placeholder").removeClass("select2-selection__choice"), n
        }, e.prototype.update = function (e, t) {
            var n = 1 == t.length && t[0].id != this.placeholder.id,
                    r = t.length > 1;
            if (r || n) return e.call(this, t);
            this.clear();
            var i = this.createPlaceholder(this.placeholder);
            this.$selection.find(".select2-selection__rendered").append(i)
        }, e
    }), e("select2/selection/allowClear", ["jquery"], function (e) {
        function t() { }
        return t.prototype.bind = function (t, n, r) {
            var i = this;
            t.call(this, n, r), this.$selection.on("mousedown", ".select2-selection__clear", function (t) {
                if (!i.options.get("disabled")) {
                    t.stopPropagation();
                    for (var n = e(this).data("data"), r = 0; r < n.length; r++) {
                        var o = {
                            data: n[r]
                        };
                        if (i.trigger("unselect", o), o.prevented) return
                    }
                    i.$element.val(i.placeholder.id).trigger("change"), i.trigger("toggle")
                }
            })
        }, t.prototype.update = function (t, n) {
            if (t.call(this, n), !(this.$selection.find(".select2-selection__placeholder").length > 0 || 0 === n.length)) {
                var r = e('<span class="select2-selection__clear">&times;</span>');
                r.data("data", n), this.$selection.find(".select2-selection__rendered").append(r)
            }
        }, t
    }), e("select2/selection/search", ["jquery", "../utils", "../keys"], function (e, t, n) {
        function r(e, t, n) {
            e.call(this, t, n)
        }
        return r.prototype.render = function (t) {
            var n = e('<li class="select2-search select2-search--inline"><input class="select2-search__field" type="search" tabindex="-1" role="textbox" /></li>');
            this.$searchContainer = n, this.$search = n.find("input");
            var r = t.call(this);
            return r
        }, r.prototype.bind = function (e, t, r) {
            var i = this;
            e.call(this, t, r), t.on("open", function () {
                i.$search.attr("tabindex", 0), i.$search.focus()
            }), t.on("close", function () {
                i.$search.attr("tabindex", -1), i.$search.val("")
            }), t.on("enable", function () {
                i.$search.prop("disabled", !1)
            }), t.on("disable", function () {
                i.$search.prop("disabled", !0)
            }), this.$selection.on("keydown", ".select2-search--inline", function (e) {
                e.stopPropagation(), i.trigger("keypress", e), i._keyUpPrevented = e.isDefaultPrevented();
                var t = e.which;
                if (t === n.BACKSPACE && "" === i.$search.val()) {
                    var r = i.$searchContainer.prev(".select2-selection__choice");
                    if (r.length > 0) {
                        var o = r.data("data");
                        i.searchRemoveChoice(o)
                    }
                }
            }), this.$selection.on("keyup", ".select2-search--inline", function (e) {
                i.handleSearch(e)
            })
        }, r.prototype.createPlaceholder = function (e, t) {
            this.$search.attr("placeholder", t.text)
        }, r.prototype.update = function (e, t) {
            this.$search.attr("placeholder", ""), e.call(this, t), this.$selection.find(".select2-selection__rendered").append(this.$searchContainer), this.resizeSearch()
        }, r.prototype.handleSearch = function () {
            if (this.resizeSearch(), !this._keyUpPrevented) {
                var e = this.$search.val();
                this.trigger("query", {
                    term: e
                })
            }
            this._keyUpPrevented = !1
        }, r.prototype.searchRemoveChoice = function (e, t) {
            this.trigger("unselect", {
                data: t
            }), this.trigger("open"), this.$search.val(t.text + " ")
        }, r.prototype.resizeSearch = function () {
            this.$search.css("width", "25px");
            var e = "";
            if ("" !== this.$search.attr("placeholder")) e = this.$selection.find(".select2-selection__rendered").innerWidth();
            else {
                var t = this.$search.val().length + 1;
                e = .75 * t + "em"
            }
            this.$search.css("width", e)
        }, r
    }), e("select2/selection/eventRelay", ["jquery"], function (e) {
        function t() { }
        return t.prototype.bind = function (t, n, r) {
            var i = this,
                    o = ["open", "opening", "close", "closing", "select", "selecting", "unselect", "unselecting"],
                    s = ["opening", "closing", "selecting", "unselecting"];
            t.call(this, n, r), n.on("*", function (t, n) {
                if (-1 !== o.indexOf(t)) {
                    n = n || {};
                    var r = e.Event("select2:" + t, {
                        params: n
                    });
                    i.$element.trigger(r), -1 !== s.indexOf(t) && (n.prevented = r.isDefaultPrevented())
                }
            })
        }, t
    }), e("select2/translation", ["jquery"], function (e) {
        function n(e) {
            this.dict = e || {}
        }
        return n.prototype.all = function () {
            return this.dict
        }, n.prototype.get = function (e) {
            return this.dict[e]
        }, n.prototype.extend = function (t) {
            this.dict = e.extend({}, t.all(), this.dict)
        }, n._cache = {}, n.loadPath = function (e) {
            if (!(e in n._cache)) {
                var r = t(e);
                n._cache[e] = r
            }
            return new n(n._cache[e])
        }, n
    }), e("select2/diacritics", [], function () {
        var e = {
            "Ⓐ": "A",
            "Ａ": "A",
            "À": "A",
            "Á": "A",
            "Â": "A",
            "Ầ": "A",
            "Ấ": "A",
            "Ẫ": "A",
            "Ẩ": "A",
            "Ã": "A",
            "Ā": "A",
            "Ă": "A",
            "Ằ": "A",
            "Ắ": "A",
            "Ẵ": "A",
            "Ẳ": "A",
            "Ȧ": "A",
            "Ǡ": "A",
            "Ä": "A",
            "Ǟ": "A",
            "Ả": "A",
            "Å": "A",
            "Ǻ": "A",
            "Ǎ": "A",
            "Ȁ": "A",
            "Ȃ": "A",
            "Ạ": "A",
            "Ậ": "A",
            "Ặ": "A",
            "Ḁ": "A",
            "Ą": "A",
            "Ⱥ": "A",
            "Ɐ": "A",
            "Ꜳ": "AA",
            "Æ": "AE",
            "Ǽ": "AE",
            "Ǣ": "AE",
            "Ꜵ": "AO",
            "Ꜷ": "AU",
            "Ꜹ": "AV",
            "Ꜻ": "AV",
            "Ꜽ": "AY",
            "Ⓑ": "B",
            "Ｂ": "B",
            "Ḃ": "B",
            "Ḅ": "B",
            "Ḇ": "B",
            "Ƀ": "B",
            "Ƃ": "B",
            "Ɓ": "B",
            "Ⓒ": "C",
            "Ｃ": "C",
            "Ć": "C",
            "Ĉ": "C",
            "Ċ": "C",
            "Č": "C",
            "Ç": "C",
            "Ḉ": "C",
            "Ƈ": "C",
            "Ȼ": "C",
            "Ꜿ": "C",
            "Ⓓ": "D",
            "Ｄ": "D",
            "Ḋ": "D",
            "Ď": "D",
            "Ḍ": "D",
            "Ḑ": "D",
            "Ḓ": "D",
            "Ḏ": "D",
            "Đ": "D",
            "Ƌ": "D",
            "Ɗ": "D",
            "Ɖ": "D",
            "Ꝺ": "D",
            "Ǳ": "DZ",
            "Ǆ": "DZ",
            "ǲ": "Dz",
            "ǅ": "Dz",
            "Ⓔ": "E",
            "Ｅ": "E",
            "È": "E",
            "É": "E",
            "Ê": "E",
            "Ề": "E",
            "Ế": "E",
            "Ễ": "E",
            "Ể": "E",
            "Ẽ": "E",
            "Ē": "E",
            "Ḕ": "E",
            "Ḗ": "E",
            "Ĕ": "E",
            "Ė": "E",
            "Ë": "E",
            "Ẻ": "E",
            "Ě": "E",
            "Ȅ": "E",
            "Ȇ": "E",
            "Ẹ": "E",
            "Ệ": "E",
            "Ȩ": "E",
            "Ḝ": "E",
            "Ę": "E",
            "Ḙ": "E",
            "Ḛ": "E",
            "Ɛ": "E",
            "Ǝ": "E",
            "Ⓕ": "F",
            "Ｆ": "F",
            "Ḟ": "F",
            "Ƒ": "F",
            "Ꝼ": "F",
            "Ⓖ": "G",
            "Ｇ": "G",
            "Ǵ": "G",
            "Ĝ": "G",
            "Ḡ": "G",
            "Ğ": "G",
            "Ġ": "G",
            "Ǧ": "G",
            "Ģ": "G",
            "Ǥ": "G",
            "Ɠ": "G",
            "Ꞡ": "G",
            "Ᵹ": "G",
            "Ꝿ": "G",
            "Ⓗ": "H",
            "Ｈ": "H",
            "Ĥ": "H",
            "Ḣ": "H",
            "Ḧ": "H",
            "Ȟ": "H",
            "Ḥ": "H",
            "Ḩ": "H",
            "Ḫ": "H",
            "Ħ": "H",
            "Ⱨ": "H",
            "Ⱶ": "H",
            "Ɥ": "H",
            "Ⓘ": "I",
            "Ｉ": "I",
            "Ì": "I",
            "Í": "I",
            "Î": "I",
            "Ĩ": "I",
            "Ī": "I",
            "Ĭ": "I",
            "İ": "I",
            "Ï": "I",
            "Ḯ": "I",
            "Ỉ": "I",
            "Ǐ": "I",
            "Ȉ": "I",
            "Ȋ": "I",
            "Ị": "I",
            "Į": "I",
            "Ḭ": "I",
            "Ɨ": "I",
            "Ⓙ": "J",
            "Ｊ": "J",
            "Ĵ": "J",
            "Ɉ": "J",
            "Ⓚ": "K",
            "Ｋ": "K",
            "Ḱ": "K",
            "Ǩ": "K",
            "Ḳ": "K",
            "Ķ": "K",
            "Ḵ": "K",
            "Ƙ": "K",
            "Ⱪ": "K",
            "Ꝁ": "K",
            "Ꝃ": "K",
            "Ꝅ": "K",
            "Ꞣ": "K",
            "Ⓛ": "L",
            "Ｌ": "L",
            "Ŀ": "L",
            "Ĺ": "L",
            "Ľ": "L",
            "Ḷ": "L",
            "Ḹ": "L",
            "Ļ": "L",
            "Ḽ": "L",
            "Ḻ": "L",
            "Ł": "L",
            "Ƚ": "L",
            "Ɫ": "L",
            "Ⱡ": "L",
            "Ꝉ": "L",
            "Ꝇ": "L",
            "Ꞁ": "L",
            "Ǉ": "LJ",
            "ǈ": "Lj",
            "Ⓜ": "M",
            "Ｍ": "M",
            "Ḿ": "M",
            "Ṁ": "M",
            "Ṃ": "M",
            "Ɱ": "M",
            "Ɯ": "M",
            "Ⓝ": "N",
            "Ｎ": "N",
            "Ǹ": "N",
            "Ń": "N",
            "Ñ": "N",
            "Ṅ": "N",
            "Ň": "N",
            "Ṇ": "N",
            "Ņ": "N",
            "Ṋ": "N",
            "Ṉ": "N",
            "Ƞ": "N",
            "Ɲ": "N",
            "Ꞑ": "N",
            "Ꞥ": "N",
            "Ǌ": "NJ",
            "ǋ": "Nj",
            "Ⓞ": "O",
            "Ｏ": "O",
            "Ò": "O",
            "Ó": "O",
            "Ô": "O",
            "Ồ": "O",
            "Ố": "O",
            "Ỗ": "O",
            "Ổ": "O",
            "Õ": "O",
            "Ṍ": "O",
            "Ȭ": "O",
            "Ṏ": "O",
            "Ō": "O",
            "Ṑ": "O",
            "Ṓ": "O",
            "Ŏ": "O",
            "Ȯ": "O",
            "Ȱ": "O",
            "Ö": "O",
            "Ȫ": "O",
            "Ỏ": "O",
            "Ő": "O",
            "Ǒ": "O",
            "Ȍ": "O",
            "Ȏ": "O",
            "Ơ": "O",
            "Ờ": "O",
            "Ớ": "O",
            "Ỡ": "O",
            "Ở": "O",
            "Ợ": "O",
            "Ọ": "O",
            "Ộ": "O",
            "Ǫ": "O",
            "Ǭ": "O",
            "Ø": "O",
            "Ǿ": "O",
            "Ɔ": "O",
            "Ɵ": "O",
            "Ꝋ": "O",
            "Ꝍ": "O",
            "Ƣ": "OI",
            "Ꝏ": "OO",
            "Ȣ": "OU",
            "Ⓟ": "P",
            "Ｐ": "P",
            "Ṕ": "P",
            "Ṗ": "P",
            "Ƥ": "P",
            "Ᵽ": "P",
            "Ꝑ": "P",
            "Ꝓ": "P",
            "Ꝕ": "P",
            "Ⓠ": "Q",
            "Ｑ": "Q",
            "Ꝗ": "Q",
            "Ꝙ": "Q",
            "Ɋ": "Q",
            "Ⓡ": "R",
            "Ｒ": "R",
            "Ŕ": "R",
            "Ṙ": "R",
            "Ř": "R",
            "Ȑ": "R",
            "Ȓ": "R",
            "Ṛ": "R",
            "Ṝ": "R",
            "Ŗ": "R",
            "Ṟ": "R",
            "Ɍ": "R",
            "Ɽ": "R",
            "Ꝛ": "R",
            "Ꞧ": "R",
            "Ꞃ": "R",
            "Ⓢ": "S",
            "Ｓ": "S",
            "ẞ": "S",
            "Ś": "S",
            "Ṥ": "S",
            "Ŝ": "S",
            "Ṡ": "S",
            "Š": "S",
            "Ṧ": "S",
            "Ṣ": "S",
            "Ṩ": "S",
            "Ș": "S",
            "Ş": "S",
            "Ȿ": "S",
            "Ꞩ": "S",
            "Ꞅ": "S",
            "Ⓣ": "T",
            "Ｔ": "T",
            "Ṫ": "T",
            "Ť": "T",
            "Ṭ": "T",
            "Ț": "T",
            "Ţ": "T",
            "Ṱ": "T",
            "Ṯ": "T",
            "Ŧ": "T",
            "Ƭ": "T",
            "Ʈ": "T",
            "Ⱦ": "T",
            "Ꞇ": "T",
            "Ꜩ": "TZ",
            "Ⓤ": "U",
            "Ｕ": "U",
            "Ù": "U",
            "Ú": "U",
            "Û": "U",
            "Ũ": "U",
            "Ṹ": "U",
            "Ū": "U",
            "Ṻ": "U",
            "Ŭ": "U",
            "Ü": "U",
            "Ǜ": "U",
            "Ǘ": "U",
            "Ǖ": "U",
            "Ǚ": "U",
            "Ủ": "U",
            "Ů": "U",
            "Ű": "U",
            "Ǔ": "U",
            "Ȕ": "U",
            "Ȗ": "U",
            "Ư": "U",
            "Ừ": "U",
            "Ứ": "U",
            "Ữ": "U",
            "Ử": "U",
            "Ự": "U",
            "Ụ": "U",
            "Ṳ": "U",
            "Ų": "U",
            "Ṷ": "U",
            "Ṵ": "U",
            "Ʉ": "U",
            "Ⓥ": "V",
            "Ｖ": "V",
            "Ṽ": "V",
            "Ṿ": "V",
            "Ʋ": "V",
            "Ꝟ": "V",
            "Ʌ": "V",
            "Ꝡ": "VY",
            "Ⓦ": "W",
            "Ｗ": "W",
            "Ẁ": "W",
            "Ẃ": "W",
            "Ŵ": "W",
            "Ẇ": "W",
            "Ẅ": "W",
            "Ẉ": "W",
            "Ⱳ": "W",
            "Ⓧ": "X",
            "Ｘ": "X",
            "Ẋ": "X",
            "Ẍ": "X",
            "Ⓨ": "Y",
            "Ｙ": "Y",
            "Ỳ": "Y",
            "Ý": "Y",
            "Ŷ": "Y",
            "Ỹ": "Y",
            "Ȳ": "Y",
            "Ẏ": "Y",
            "Ÿ": "Y",
            "Ỷ": "Y",
            "Ỵ": "Y",
            "Ƴ": "Y",
            "Ɏ": "Y",
            "Ỿ": "Y",
            "Ⓩ": "Z",
            "Ｚ": "Z",
            "Ź": "Z",
            "Ẑ": "Z",
            "Ż": "Z",
            "Ž": "Z",
            "Ẓ": "Z",
            "Ẕ": "Z",
            "Ƶ": "Z",
            "Ȥ": "Z",
            "Ɀ": "Z",
            "Ⱬ": "Z",
            "Ꝣ": "Z",
            "ⓐ": "a",
            "ａ": "a",
            "ẚ": "a",
            "à": "a",
            "á": "a",
            "â": "a",
            "ầ": "a",
            "ấ": "a",
            "ẫ": "a",
            "ẩ": "a",
            "ã": "a",
            "ā": "a",
            "ă": "a",
            "ằ": "a",
            "ắ": "a",
            "ẵ": "a",
            "ẳ": "a",
            "ȧ": "a",
            "ǡ": "a",
            "ä": "a",
            "ǟ": "a",
            "ả": "a",
            "å": "a",
            "ǻ": "a",
            "ǎ": "a",
            "ȁ": "a",
            "ȃ": "a",
            "ạ": "a",
            "ậ": "a",
            "ặ": "a",
            "ḁ": "a",
            "ą": "a",
            "ⱥ": "a",
            "ɐ": "a",
            "ꜳ": "aa",
            "æ": "ae",
            "ǽ": "ae",
            "ǣ": "ae",
            "ꜵ": "ao",
            "ꜷ": "au",
            "ꜹ": "av",
            "ꜻ": "av",
            "ꜽ": "ay",
            "ⓑ": "b",
            "ｂ": "b",
            "ḃ": "b",
            "ḅ": "b",
            "ḇ": "b",
            "ƀ": "b",
            "ƃ": "b",
            "ɓ": "b",
            "ⓒ": "c",
            "ｃ": "c",
            "ć": "c",
            "ĉ": "c",
            "ċ": "c",
            "č": "c",
            "ç": "c",
            "ḉ": "c",
            "ƈ": "c",
            "ȼ": "c",
            "ꜿ": "c",
            "ↄ": "c",
            "ⓓ": "d",
            "ｄ": "d",
            "ḋ": "d",
            "ď": "d",
            "ḍ": "d",
            "ḑ": "d",
            "ḓ": "d",
            "ḏ": "d",
            "đ": "d",
            "ƌ": "d",
            "ɖ": "d",
            "ɗ": "d",
            "ꝺ": "d",
            "ǳ": "dz",
            "ǆ": "dz",
            "ⓔ": "e",
            "ｅ": "e",
            "è": "e",
            "é": "e",
            "ê": "e",
            "ề": "e",
            "ế": "e",
            "ễ": "e",
            "ể": "e",
            "ẽ": "e",
            "ē": "e",
            "ḕ": "e",
            "ḗ": "e",
            "ĕ": "e",
            "ė": "e",
            "ë": "e",
            "ẻ": "e",
            "ě": "e",
            "ȅ": "e",
            "ȇ": "e",
            "ẹ": "e",
            "ệ": "e",
            "ȩ": "e",
            "ḝ": "e",
            "ę": "e",
            "ḙ": "e",
            "ḛ": "e",
            "ɇ": "e",
            "ɛ": "e",
            "ǝ": "e",
            "ⓕ": "f",
            "ｆ": "f",
            "ḟ": "f",
            "ƒ": "f",
            "ꝼ": "f",
            "ⓖ": "g",
            "ｇ": "g",
            "ǵ": "g",
            "ĝ": "g",
            "ḡ": "g",
            "ğ": "g",
            "ġ": "g",
            "ǧ": "g",
            "ģ": "g",
            "ǥ": "g",
            "ɠ": "g",
            "ꞡ": "g",
            "ᵹ": "g",
            "ꝿ": "g",
            "ⓗ": "h",
            "ｈ": "h",
            "ĥ": "h",
            "ḣ": "h",
            "ḧ": "h",
            "ȟ": "h",
            "ḥ": "h",
            "ḩ": "h",
            "ḫ": "h",
            "ẖ": "h",
            "ħ": "h",
            "ⱨ": "h",
            "ⱶ": "h",
            "ɥ": "h",
            "ƕ": "hv",
            "ⓘ": "i",
            "ｉ": "i",
            "ì": "i",
            "í": "i",
            "î": "i",
            "ĩ": "i",
            "ī": "i",
            "ĭ": "i",
            "ï": "i",
            "ḯ": "i",
            "ỉ": "i",
            "ǐ": "i",
            "ȉ": "i",
            "ȋ": "i",
            "ị": "i",
            "į": "i",
            "ḭ": "i",
            "ɨ": "i",
            "ı": "i",
            "ⓙ": "j",
            "ｊ": "j",
            "ĵ": "j",
            "ǰ": "j",
            "ɉ": "j",
            "ⓚ": "k",
            "ｋ": "k",
            "ḱ": "k",
            "ǩ": "k",
            "ḳ": "k",
            "ķ": "k",
            "ḵ": "k",
            "ƙ": "k",
            "ⱪ": "k",
            "ꝁ": "k",
            "ꝃ": "k",
            "ꝅ": "k",
            "ꞣ": "k",
            "ⓛ": "l",
            "ｌ": "l",
            "ŀ": "l",
            "ĺ": "l",
            "ľ": "l",
            "ḷ": "l",
            "ḹ": "l",
            "ļ": "l",
            "ḽ": "l",
            "ḻ": "l",
            "ſ": "l",
            "ł": "l",
            "ƚ": "l",
            "ɫ": "l",
            "ⱡ": "l",
            "ꝉ": "l",
            "ꞁ": "l",
            "ꝇ": "l",
            "ǉ": "lj",
            "ⓜ": "m",
            "ｍ": "m",
            "ḿ": "m",
            "ṁ": "m",
            "ṃ": "m",
            "ɱ": "m",
            "ɯ": "m",
            "ⓝ": "n",
            "ｎ": "n",
            "ǹ": "n",
            "ń": "n",
            "ñ": "n",
            "ṅ": "n",
            "ň": "n",
            "ṇ": "n",
            "ņ": "n",
            "ṋ": "n",
            "ṉ": "n",
            "ƞ": "n",
            "ɲ": "n",
            "ŉ": "n",
            "ꞑ": "n",
            "ꞥ": "n",
            "ǌ": "nj",
            "ⓞ": "o",
            "ｏ": "o",
            "ò": "o",
            "ó": "o",
            "ô": "o",
            "ồ": "o",
            "ố": "o",
            "ỗ": "o",
            "ổ": "o",
            "õ": "o",
            "ṍ": "o",
            "ȭ": "o",
            "ṏ": "o",
            "ō": "o",
            "ṑ": "o",
            "ṓ": "o",
            "ŏ": "o",
            "ȯ": "o",
            "ȱ": "o",
            "ö": "o",
            "ȫ": "o",
            "ỏ": "o",
            "ő": "o",
            "ǒ": "o",
            "ȍ": "o",
            "ȏ": "o",
            "ơ": "o",
            "ờ": "o",
            "ớ": "o",
            "ỡ": "o",
            "ở": "o",
            "ợ": "o",
            "ọ": "o",
            "ộ": "o",
            "ǫ": "o",
            "ǭ": "o",
            "ø": "o",
            "ǿ": "o",
            "ɔ": "o",
            "ꝋ": "o",
            "ꝍ": "o",
            "ɵ": "o",
            "ƣ": "oi",
            "ȣ": "ou",
            "ꝏ": "oo",
            "ⓟ": "p",
            "ｐ": "p",
            "ṕ": "p",
            "ṗ": "p",
            "ƥ": "p",
            "ᵽ": "p",
            "ꝑ": "p",
            "ꝓ": "p",
            "ꝕ": "p",
            "ⓠ": "q",
            "ｑ": "q",
            "ɋ": "q",
            "ꝗ": "q",
            "ꝙ": "q",
            "ⓡ": "r",
            "ｒ": "r",
            "ŕ": "r",
            "ṙ": "r",
            "ř": "r",
            "ȑ": "r",
            "ȓ": "r",
            "ṛ": "r",
            "ṝ": "r",
            "ŗ": "r",
            "ṟ": "r",
            "ɍ": "r",
            "ɽ": "r",
            "ꝛ": "r",
            "ꞧ": "r",
            "ꞃ": "r",
            "ⓢ": "s",
            "ｓ": "s",
            "ß": "s",
            "ś": "s",
            "ṥ": "s",
            "ŝ": "s",
            "ṡ": "s",
            "š": "s",
            "ṧ": "s",
            "ṣ": "s",
            "ṩ": "s",
            "ș": "s",
            "ş": "s",
            "ȿ": "s",
            "ꞩ": "s",
            "ꞅ": "s",
            "ẛ": "s",
            "ⓣ": "t",
            "ｔ": "t",
            "ṫ": "t",
            "ẗ": "t",
            "ť": "t",
            "ṭ": "t",
            "ț": "t",
            "ţ": "t",
            "ṱ": "t",
            "ṯ": "t",
            "ŧ": "t",
            "ƭ": "t",
            "ʈ": "t",
            "ⱦ": "t",
            "ꞇ": "t",
            "ꜩ": "tz",
            "ⓤ": "u",
            "ｕ": "u",
            "ù": "u",
            "ú": "u",
            "û": "u",
            "ũ": "u",
            "ṹ": "u",
            "ū": "u",
            "ṻ": "u",
            "ŭ": "u",
            "ü": "u",
            "ǜ": "u",
            "ǘ": "u",
            "ǖ": "u",
            "ǚ": "u",
            "ủ": "u",
            "ů": "u",
            "ű": "u",
            "ǔ": "u",
            "ȕ": "u",
            "ȗ": "u",
            "ư": "u",
            "ừ": "u",
            "ứ": "u",
            "ữ": "u",
            "ử": "u",
            "ự": "u",
            "ụ": "u",
            "ṳ": "u",
            "ų": "u",
            "ṷ": "u",
            "ṵ": "u",
            "ʉ": "u",
            "ⓥ": "v",
            "ｖ": "v",
            "ṽ": "v",
            "ṿ": "v",
            "ʋ": "v",
            "ꝟ": "v",
            "ʌ": "v",
            "ꝡ": "vy",
            "ⓦ": "w",
            "ｗ": "w",
            "ẁ": "w",
            "ẃ": "w",
            "ŵ": "w",
            "ẇ": "w",
            "ẅ": "w",
            "ẘ": "w",
            "ẉ": "w",
            "ⱳ": "w",
            "ⓧ": "x",
            "ｘ": "x",
            "ẋ": "x",
            "ẍ": "x",
            "ⓨ": "y",
            "ｙ": "y",
            "ỳ": "y",
            "ý": "y",
            "ŷ": "y",
            "ỹ": "y",
            "ȳ": "y",
            "ẏ": "y",
            "ÿ": "y",
            "ỷ": "y",
            "ẙ": "y",
            "ỵ": "y",
            "ƴ": "y",
            "ɏ": "y",
            "ỿ": "y",
            "ⓩ": "z",
            "ｚ": "z",
            "ź": "z",
            "ẑ": "z",
            "ż": "z",
            "ž": "z",
            "ẓ": "z",
            "ẕ": "z",
            "ƶ": "z",
            "ȥ": "z",
            "ɀ": "z",
            "ⱬ": "z",
            "ꝣ": "z",
            "Ά": "Α",
            "Έ": "Ε",
            "Ή": "Η",
            "Ί": "Ι",
            "Ϊ": "Ι",
            "Ό": "Ο",
            "Ύ": "Υ",
            "Ϋ": "Υ",
            "Ώ": "Ω",
            "ά": "α",
            "έ": "ε",
            "ή": "η",
            "ί": "ι",
            "ϊ": "ι",
            "ΐ": "ι",
            "ό": "ο",
            "ύ": "υ",
            "ϋ": "υ",
            "ΰ": "υ",
            "ω": "ω",
            "ς": "σ"
        };
        return e
    }), e("select2/data/base", ["../utils"], function (e) {
        function t() {
            t.__super__.constructor.call(this)
        }
        return e.Extend(t, e.Observable), t.prototype.current = function () {
            throw new Error("The `current` method must be defined in child classes.")
        }, t.prototype.query = function () {
            throw new Error("The `query` method must be defined in child classes.")
        }, t.prototype.bind = function () { }, t.prototype.destroy = function () { }, t.prototype.generateResultId = function (t, n) {
            var r = t.id + "-result-";
            return r += e.generateChars(4), r += null != n.id ? "-" + n.id.toString() : "-" + e.generateChars(4)
        }, t
    }), e("select2/data/select", ["./base", "../utils", "jquery"], function (e, t, n) {
        function r(e, t) {
            this.$element = e, this.options = t, r.__super__.constructor.call(this)
        }
        return t.Extend(r, e), r.prototype.current = function (e) {
            var t = [],
                    r = this;
            this.$element.find(":selected").each(function () {
                var e = n(this),
                        i = r.item(e);
                t.push(i)
            }), e(t)
        }, r.prototype.select = function (e) {
            var t = this;
            if (n(e.element).is("option")) return e.element.selected = !0, void this.$element.trigger("change");
            if (this.$element.prop("multiple")) this.current(function (n) {
                var r = [];
                e = [e], e.push.apply(e, n);
                for (var i = 0; i < e.length; i++) id = e[i].id, -1 === r.indexOf(id) && r.push(id);
                t.$element.val(r), t.$element.trigger("change")
            });
            else {
                var r = e.id;
                this.$element.val(r), this.$element.trigger("change")
            }
        }, r.prototype.unselect = function (e) {
            var t = this;
            if (this.$element.prop("multiple")) return n(e.element).is("option") ? (e.element.selected = !1, void this.$element.trigger("change")) : void this.current(function (n) {
                for (var r = [], i = 0; i < n.length; i++) id = n[i].id, id !== e.id && -1 === r.indexOf(id) && r.push(id);
                t.$element.val(r), t.$element.trigger("change")
            })
        }, r.prototype.bind = function (e) {
            var t = this;
            this.container = e, e.on("select", function (e) {
                t.select(e.data)
            }), e.on("unselect", function (e) {
                t.unselect(e.data)
            })
        }, r.prototype.destroy = function () {
            this.$element.find("*").each(function () {
                n.removeData(this, "data")
            })
        }, r.prototype.query = function (e, t) {
            var r = [],
                    i = this,
                    o = this.$element.children();
            o.each(function () {
                var t = n(this);
                if (t.is("option") || t.is("optgroup")) {
                    var o = i.item(t),
                            s = i.matches(e, o);
                    null !== s && r.push(s)
                }
            }), t({
                results: r
            })
        }, r.prototype.option = function (e) {
            var t;
            e.children ? (t = document.createElement("optgroup"), t.label = e.text) : (t = document.createElement("option"), t.innerText = e.text), e.id && (t.value = e.id), e.disabled && (t.disabled = !0), e.selected && (t.selected = !0);
            var r = n(t),
                    i = this._normalizeItem(e);
            return i.element = t, n.data(t, "data", i), r
        }, r.prototype.item = function (e) {
            var t = {};
            if (t = n.data(e[0], "data"), null != t) return t;
            if (e.is("option")) t = {
                id: e.val(),
                text: e.html(),
                disabled: e.prop("disabled"),
                selected: e.prop("selected")
            };
            else if (e.is("optgroup")) {
                t = {
                    text: e.prop("label"),
                    children: []
                };
                for (var r = e.children("option"), i = [], o = 0; o < r.length; o++) {
                    var s = n(r[o]),
                            a = this.item(s);
                    i.push(a)
                }
                t.children = i
            }
            return t = this._normalizeItem(t), t.element = e[0], n.data(e[0], "data", t), t
        }, r.prototype._normalizeItem = function (e) {
            n.isPlainObject(e) || (e = {
                id: e,
                text: e
            }), e = n.extend({}, {
                text: ""
            }, e);
            var t = {
                selected: !1,
                disabled: !1
            };
            return null != e.id && (e.id = e.id.toString()), null != e.text && (e.text = e.text.toString()), null == e._resultId && e.id && null != this.container && (e._resultId = this.generateResultId(this.container, e)), n.extend({}, t, e)
        }, r.prototype.matches = function (e, t) {
            var n = this.options.get("matcher");
            return n(e, t)
        }, r
    }), e("select2/data/array", ["./select", "../utils", "jquery"], function (e, t, n) {
        function r(e, t) {
            var n = t.get("data") || [];
            r.__super__.constructor.call(this, e, t), e.append(this.convertToOptions(n))
        }
        return t.Extend(r, e), r.prototype.select = function (e) {
            var t = this.$element.find('option[value="' + e.id + '"]');
            0 === t.length && (t = this.option(e), this.$element.append(t)), r.__super__.select.call(this, e)
        }, r.prototype.convertToOptions = function (e) {
            function t(e) {
                return function () {
                    return n(this).val() == e.id
                }
            }
            for (var r = this, i = this.$element.find("option"), o = i.map(function () {
                return r.item(n(this)).id
            }).get(), s = [], a = 0; a < e.length; a++) {
                var l = this._normalizeItem(e[a]);
                if (o.indexOf(l.id) >= 0) {
                    var c = i.filter(t(l)),
                            u = this.item(c),
                            d = (n.extend(!0, {}, u, l), this.option(u));
                    c.replaceWith(d)
                } else {
                    var h = this.option(l);
                    if (l.children) {
                        var p = this.convertToOptions(l.children);
                        h.append(p)
                    }
                    s.push(h)
                }
            }
            return s
        }, r
    }), e("select2/data/ajax", ["./array", "../utils", "jquery"], function (e, t, n) {
        function r(t, n) {
            this.ajaxOptions = n.get("ajax"), null != this.ajaxOptions.processResults && (this.processResults = this.ajaxOptions.processResults), e.__super__.constructor.call(this, t, n)
        }
        return t.Extend(r, e), r.prototype.processResults = function (e) {
            return e
        }, r.prototype.query = function (e, t) {
            function r() {
                var r = n.ajax(o);
                r.success(function (r) {
                    var o = i.processResults(r, e);
                    console && console.error && (o && o.results && n.isArray(o.results) || console.error("Select2: The AJAX results did not return an array in the `results` key of the response.")), t(o)
                }), i._request = r
            }
            var i = this;
            this._request && (this._request.abort(), this._request = null);
            var o = n.extend({
                type: "GET"
            }, this.ajaxOptions);
            "function" == typeof o.url && (o.url = o.url(e)), "function" == typeof o.data && (o.data = o.data(e)), this.ajaxOptions.delay && "" !== e.term ? (this._queryTimeout && window.clearTimeout(this._queryTimeout), this._queryTimeout = window.setTimeout(r, this.ajaxOptions.delay)) : r()
        }, r
    }), e("select2/data/tags", ["jquery"], function (e) {
        function t(t, n, r) {
            var i = r.get("tags"),
                    o = r.get("createTag");
            if (void 0 !== o && (this.createTag = o), t.call(this, n, r), e.isArray(i))
                for (var s = 0; s < i.length; s++) {
                    var a = i[s],
                            l = this._normalizeItem(a),
                            c = this.option(l);
                    this.$element.append(c)
                }
        }
        return t.prototype.query = function (e, t, n) {
            function r(e, o) {
                for (var s = e.results, a = 0; a < s.length; a++) {
                    var l = s[a],
                            c = null != l.children && !r({
                                results: l.children
                            }, !0),
                            u = l.text === t.term;
                    if (u || c) return o ? !1 : (e.data = s, void n(e))
                }
                if (o) return !0;
                var d = i.createTag(t);
                if (null != d) {
                    var h = i.option(d);
                    h.attr("data-select2-tag", !0), i.$element.append(h), i.insertTag(s, d)
                }
                e.results = s, n(e)
            }
            var i = this;
            return this._removeOldTags(), null == t.term || "" === t.term || null != t.page ? void e.call(this, t, n) : void e.call(this, t, r)
        }, t.prototype.createTag = function (e, t) {
            return {
                id: t.term,
                text: t.term
            }
        }, t.prototype.insertTag = function (e, t, n) {
            t.unshift(n)
        }, t.prototype._removeOldTags = function () {
            var t = (this._lastTag, this.$element.find("option[data-select2-tag]"));
            t.each(function () {
                this.selected || e(this).remove()
            })
        }, t
    }), e("select2/data/tokenizer", ["jquery"], function (e) {
        function t(e, t, n) {
            var r = n.get("tokenizer");
            void 0 !== r && (this.tokenizer = r), e.call(this, t, n)
        }
        return t.prototype.bind = function (e, t, n) {
            e.call(this, t, n), this.$search = t.dropdown.$search || t.selection.$search || n.find(".select2-search__field")
        }, t.prototype.query = function (e, t, n) {
            function r(e) {
                i.select(e)
            }
            var i = this;
            t.term = t.term || "";
            var o = this.tokenizer(t, this.options, r);
            o.term !== t.term && (this.$search.length && (this.$search.val(o.term), this.$search.focus()), t.term = o.term), e.call(this, t, n)
        }, t.prototype.tokenizer = function (t, n, r, i) {
            for (var o = r.get("tokenSeparators") || [], s = n.term, a = 0, l = this.createTag || function (e) {
                return {
                    id: e.term,
                    text: e.term
                }
            }; a < s.length; ) {
                var c = s[a];
                if (-1 !== o.indexOf(c)) {
                    var u = s.substr(0, a),
                            d = e.extend({}, n, {
                                term: u
                            }),
                            h = l(d);
                    i(h), s = s.substr(a + 1) || "", a = 0
                } else a++
            }
            return {
                term: s
            }
        }, t
    }), e("select2/data/minimumInputLength", [], function () {
        function e(e, t, n) {
            this.minimumInputLength = n.get("minimumInputLength"), e.call(this, t, n)
        }
        return e.prototype.query = function (e, t, n) {
            return t.term = t.term || "", t.term.length < this.minimumInputLength ? void this.trigger("results:message", {
                message: "inputTooShort",
                args: {
                    minimum: this.minimumInputLength,
                    input: t.term,
                    params: t
                }
            }) : void e.call(this, t, n)
        }, e
    }), e("select2/data/maximumInputLength", [], function () {
        function e(e, t, n) {
            this.maximumInputLength = n.get("maximumInputLength"), e.call(this, t, n)
        }
        return e.prototype.query = function (e, t, n) {
            return t.term = t.term || "", this.maximumInputLength > 0 && t.term.length > this.maximumInputLength ? void this.trigger("results:message", {
                message: "inputTooLong",
                args: {
                    minimum: this.maximumInputLength,
                    input: t.term,
                    params: t
                }
            }) : void e.call(this, t, n)
        }, e
    }), e("select2/data/maximumSelectionLength", [], function () {
        function e(e, t, n) {
            this.maximumSelectionLength = n.get("maximumSelectionLength"), e.call(this, t, n)
        }
        return e.prototype.query = function (e, t, n) {
            var r = this;
            this.current(function (i) {
                var o = null != i ? i.length : 0;
                return r.maximumSelectionLength > 0 && o >= r.maximumSelectionLength ? void r.trigger("results:message", {
                    message: "maximumSelected",
                    args: {
                        maximum: r.maximumSelectionLength
                    }
                }) : void e.call(r, t, n)
            })
        }, e
    }), e("select2/dropdown", ["jquery", "./utils"], function (e, t) {
        function n(e, t) {
            this.$element = e, this.options = t, n.__super__.constructor.call(this)
        }
        return t.Extend(n, t.Observable), n.prototype.render = function () {
            var t = e('<span class="select2-dropdown"><span class="select2-results"></span></span>');
            return t.attr("dir", this.options.get("dir")), this.$dropdown = t, t
        }, n.prototype.position = function () { }, n.prototype.destroy = function () {
            this.$dropdown.remove()
        }, n.prototype.bind = function (e) {
            var t = this;
            e.on("select", function (e) {
                t._onSelect(e)
            }), e.on("unselect", function (e) {
                t._onUnSelect(e)
            })
        }, n.prototype._onSelect = function () {
            this.trigger("close")
        }, n.prototype._onUnSelect = function () {
            this.trigger("close")
        }, n
    }), e("select2/dropdown/search", ["jquery", "../utils"], function (e) {
        function t() { }
        return t.prototype.render = function (t) {
            var n = t.call(this),
                    r = e('<span class="select2-search select2-search--dropdown"><input class="select2-search__field" type="search" tabindex="-1" role="textbox" /></span>');
            return this.$searchContainer = r, this.$search = r.find("input"), n.prepend(r), n
        }, t.prototype.bind = function (e, t, n) {
            var r = this;
            e.call(this, t, n), this.$search.on("keydown", function (e) {
                r.trigger("keypress", e), r._keyUpPrevented = e.isDefaultPrevented()
            }), this.$search.on("keyup", function (e) {
                r.handleSearch(e)
            }), t.on("open", function () {
                r.$search.attr("tabindex", 0), r.$search.focus(), window.setTimeout(function () {
                    r.$search.focus()
                }, 0)
            }), t.on("close", function () {
                r.$search.attr("tabindex", -1), r.$search.val("")
            }), t.on("results:all", function (e) {
                if (null == e.query.term || "" === e.query.term) {
                    var t = r.showSearch(e);
                    t ? r.$searchContainer.removeClass("select2-search--hide") : r.$searchContainer.addClass("select2-search--hide")
                }
            })
        }, t.prototype.handleSearch = function () {
            if (!this._keyUpPrevented) {
                var e = this.$search.val();
                this.trigger("query", {
                    term: e
                })
            }
            this._keyUpPrevented = !1
        }, t.prototype.showSearch = function () {
            return !0
        }, t
    }), e("select2/dropdown/hidePlaceholder", [], function () {
        function e(e, t, n, r) {
            this.placeholder = this.normalizePlaceholder(n.get("placeholder")), e.call(this, t, n, r)
        }
        return e.prototype.append = function (e, t) {
            t.results = this.removePlaceholder(t.results), e.call(this, t)
        }, e.prototype.normalizePlaceholder = function (e, t) {
            return "string" == typeof t && (t = {
                id: "",
                text: t
            }), t
        }, e.prototype.removePlaceholder = function (e, t) {
            for (var n = t.slice(0), r = t.length - 1; r >= 0; r--) {
                var i = t[r];
                this.placeholder.id === i.id && n.splice(r, 1)
            }
            return n
        }, e
    }), e("select2/dropdown/infiniteScroll", ["jquery"], function (e) {
        function t(e, t, n, r) {
            this.lastParams = {}, e.call(this, t, n, r), this.$loadingMore = this.createLoadingMore(), this.loading = !1
        }
        return t.prototype.append = function (e, t) {
            this.$loadingMore.remove(), this.loading = !1, e.call(this, t), this.showLoadingMore(t) && this.$results.append(this.$loadingMore)
        }, t.prototype.bind = function (t, n, r) {
            var i = this;
            t.call(this, n, r), n.on("query", function (e) {
                i.lastParams = e, i.loading = !0
            }), n.on("query:append", function (e) {
                i.lastParams = e, i.loading = !0
            }), this.$results.on("scroll", function () {
                var t = e.contains(document.documentElement, i.$loadingMore[0]);
                if (!i.loading && t) {
                    var n = i.$results.offset().top + i.$results.outerHeight(!1),
                            r = i.$loadingMore.offset().top + i.$loadingMore.outerHeight(!1);
                    n + 50 >= r && i.loadMore()
                }
            })
        }, t.prototype.loadMore = function () {
            this.loading = !0;
            var t = e.extend({}, {
                page: 1
            }, this.lastParams);
            t.page++, this.trigger("query:append", t)
        }, t.prototype.showLoadingMore = function (e, t) {
            return t.pagination && t.pagination.more
        }, t.prototype.createLoadingMore = function () {
            var t = e('<li class="option load-more" role="treeitem"></li>'),
                    n = this.options.get("translations").get("loadingMore");
            return t.html(n(this.lastParams)), t
        }, t
    }), e("select2/dropdown/attachBody", ["jquery", "../utils"], function (e, t) {
        function n(e, t, n) {
            this.$dropdownParent = n.get("dropdownParent") || document.body, e.call(this, t, n)
        }
        return n.prototype.bind = function (e, t, n) {
            var r = this,
                    i = !1;
            e.call(this, t, n), t.on("open", function () {
                r._showDropdown(), r._attachPositioningHandler(t), i || (i = !0, t.on("results:all", function () {
                    r._positionDropdown(), r._resizeDropdown()
                }), t.on("results:append", function () {
                    r._positionDropdown(), r._resizeDropdown()
                }))
            }), t.on("close", function () {
                r._hideDropdown(), r._detachPositioningHandler(t)
            }), this.$dropdownContainer.on("mousedown", function (e) {
                e.stopPropagation()
            })
        }, n.prototype.position = function (e, t, n) {
            t.attr("class", n.attr("class")), t.removeClass("select2"), t.addClass("select2-container--open"), t.css({
                position: "absolute",
                top: -999999
            }), this.$container = n
        }, n.prototype.render = function (t) {
            var n = e("<span></span>"),
                    r = t.call(this);
            return n.append(r), this.$dropdownContainer = n, n
        }, n.prototype._hideDropdown = function () {
            this.$dropdownContainer.detach()
        }, n.prototype._attachPositioningHandler = function (n) {
            var r = this,
                    i = "scroll.select2." + n.id,
                    o = "resize.select2." + n.id,
                    s = "orientationchange.select2." + n.id;
            $watchers = this.$container.parents().filter(t.hasScroll), $watchers.each(function () {
                e(this).data("select2-scroll-position", {
                    x: e(this).scrollLeft(),
                    y: e(this).scrollTop()
                })
            }), $watchers.on(i, function () {
                var t = e(this).data("select2-scroll-position");
                e(this).scrollTop(t.y)
            }), e(window).on(i + " " + o + " " + s, function () {
                r._positionDropdown(), r._resizeDropdown()
            })
        }, n.prototype._detachPositioningHandler = function (n) {
            var r = "scroll.select2." + n.id,
                    i = "resize.select2." + n.id,
                    o = "orientationchange.select2." + n.id;
            $watchers = this.$container.parents().filter(t.hasScroll), $watchers.off(r), e(window).off(r + " " + i + " " + o)
        }, n.prototype._positionDropdown = function () {
            var t = e(window),
                    n = this.$dropdown.hasClass("select2-dropdown--above"),
                    r = this.$dropdown.hasClass("select2-dropdown--below"),
                    i = null,
                    o = (this.$container.position(), this.$container.offset());
            o.bottom = o.top + this.$container.outerHeight(!1);
            var s = {
                height: this.$container.outerHeight(!1)
            };
            s.top = o.top, s.bottom = o.top + s.height;
            var a = {
                height: this.$dropdown.outerHeight(!1)
            },
                    l = {
                        top: t.scrollTop(),
                        bottom: t.scrollTop() + t.height()
                    },
                    c = l.top < o.top - a.height,
                    u = l.bottom > o.bottom + a.height,
                    d = {
                        left: o.left,
                        top: s.bottom
                    };
            n || r || (i = "below"), u || !c || n ? !c && u && n && (i = "below") : i = "above", ("above" == i || n && "below" !== i) && (d.top = s.top - a.height), null != i && (this.$dropdown.removeClass("select2-dropdown--below select2-dropdown--above").addClass("select2-dropdown--" + i), this.$container.removeClass("select2-container--below select2-container--above").addClass("select2-container--" + i)), this.$dropdownContainer.css(d)
        }, n.prototype._resizeDropdown = function () {
            this.$dropdownContainer.width(), this.$dropdown.css({
                width: this.$container.outerWidth(!1) + "px"
            })
        }, n.prototype._showDropdown = function () {
            this.$dropdownContainer.appendTo(this.$dropdownParent), this._positionDropdown(), this._resizeDropdown()
        }, n
    }), e("select2/dropdown/minimumResultsForSearch", [], function () {
        function e(t) {
            count = 0;
            for (var n = 0; n < t.length; n++) {
                var r = t[n];
                r.children ? count += e(r.children) : count++
            }
            return count
        }

        function t(e, t, n, r) {
            this.minimumResultsForSearch = n.get("minimumResultsForSearch"), e.call(this, t, n, r)
        }
        return t.prototype.showSearch = function (t, n) {
            return e(n.data.results) < this.minimumResultsForSearch ? !1 : t.call(this, n)
        }, t
    }), e("select2/dropdown/selectOnClose", [], function () {
        function e() { }
        return e.prototype.bind = function (e, t, n) {
            var r = this;
            e.call(this, t, n), t.on("close", function () {
                r._handleSelectOnClose()
            })
        }, e.prototype._handleSelectOnClose = function () {
            var e = this.getHighlightedResults();
            e.length < 1 || e.trigger("mouseup")
        }, e
    }), e("select2/i18n/en", [], function () {
        return {
            errorLoading: function () {
                return "The results could not be loaded."
            },
            inputTooLong: function (e) {
                var t = e.input.length - e.maximum,
                        n = "Please delete " + t + " character";
                return 1 != t && (n += "s"), n
            },
            inputTooShort: function (e) {
                var t = e.minimum - e.input.length,
                        n = "Please enter " + t + " or more characters";
                return n
            },
            loadingMore: function () {
                return "Loading more results…"
            },
            maximumSelected: function (e) {
                var t = "You can only select " + e.maximum + " item";
                return 1 != e.maximum && (t += "s"), t
            },
            noResults: function () {
                return "No results found"
            },
            searching: function () {
                return "Searching…"
            }
        }
    }), e("select2/defaults", ["jquery", "./results", "./selection/single", "./selection/multiple", "./selection/placeholder", "./selection/allowClear", "./selection/search", "./selection/eventRelay", "./utils", "./translation", "./diacritics", "./data/select", "./data/array", "./data/ajax", "./data/tags", "./data/tokenizer", "./data/minimumInputLength", "./data/maximumInputLength", "./data/maximumSelectionLength", "./dropdown", "./dropdown/search", "./dropdown/hidePlaceholder", "./dropdown/infiniteScroll", "./dropdown/attachBody", "./dropdown/minimumResultsForSearch", "./dropdown/selectOnClose", "./i18n/en"], function (e, n, r, i, o, s, a, l, c, u, d, h, p, f, g, m, v, y, w, _, $, b, x, A, O, S, E) {
        function C() {
            this.reset()
        }
        C.prototype.apply = function (d) {
            if (d = e.extend({}, this.defaults, d), null == d.dataAdapter) {
                if (d.dataAdapter = null != d.ajax ? f : null != d.data ? p : h, d.minimumInputLength > 0 && (d.dataAdapter = c.Decorate(d.dataAdapter, v)), d.maximumInputLength > 0 && (d.dataAdapter = c.Decorate(d.dataAdapter, y)), d.maximumSelectionLength > 0 && (d.dataAdapter = c.Decorate(d.dataAdapter, w)), null != d.tags && (d.dataAdapter = c.Decorate(d.dataAdapter, g)), (null != d.tokenSeparators || null != d.tokenizer) && (d.dataAdapter = c.Decorate(d.dataAdapter, m)), null != d.query) {
                    var E = t(d.amdBase + "compat/query");
                    d.dataAdapter = c.Decorate(d.dataAdapter, E)
                }
                if (null != d.initSelection) {
                    var C = t(d.amdBase + "compat/initSelection");
                    d.dataAdapter = c.Decorate(d.dataAdapter, C)
                }
            }
            if (null == d.resultsAdapter && (d.resultsAdapter = n, null != d.ajax && (d.resultsAdapter = c.Decorate(d.resultsAdapter, x)), null != d.placeholder && (d.resultsAdapter = c.Decorate(d.resultsAdapter, b)), d.selectOnClose && (d.resultsAdapter = c.Decorate(d.resultsAdapter, S))), null == d.dropdownAdapter) {
                if (d.multiple) d.dropdownAdapter = _;
                else {
                    var D = c.Decorate(_, $);
                    d.dropdownAdapter = D
                }
                d.minimumResultsForSearch > 0 && (d.dropdownAdapter = c.Decorate(d.dropdownAdapter, O)), d.dropdownAdapter = c.Decorate(d.dropdownAdapter, A)
            }
            if (null == d.selectionAdapter && (d.selectionAdapter = d.multiple ? i : r, null != d.placeholder && (d.selectionAdapter = c.Decorate(d.selectionAdapter, o), d.allowClear && (d.selectionAdapter = c.Decorate(d.selectionAdapter, s))), d.multiple && (d.selectionAdapter = c.Decorate(d.selectionAdapter, a)), d.selectionAdapter = c.Decorate(d.selectionAdapter, l)), "string" == typeof d.language)
                if (d.language.indexOf("-") > 0) {
                    var T = d.language.split("-"),
                            q = T[0];
                    d.language = [d.language, q]
                } else d.language = [d.language];
            if (e.isArray(d.language)) {
                var j = new u;
                d.language.push("en");
                for (var L = d.language, P = 0; P < L.length; P++) {
                    var I = L[P],
                            k = {};
                    try {
                        k = u.loadPath(I)
                    } catch (z) {
                        try {
                            I = this.defaults.amdLanguageBase + I, k = u.loadPath(I)
                        } catch (H) {
                            console && console.warn && console.warn('Select2: The lanugage file for "' + I + '" could not be automatically loaded. A fallback will be used instead.');
                            continue
                        }
                    }
                    j.extend(k)
                }
                d.translations = j
            } else d.translations = new u(d.language);
            return d
        }, C.prototype.reset = function () {
            function t(e) {
                function t(e) {
                    return d[e] || e
                }
                return e.replace(/[^\u0000-\u007E]/g, t)
            }

            function n(r, i) {
                if ("" === e.trim(r.term)) return i;
                if (i.children && i.children.length > 0) {
                    for (var o = e.extend(!0, {}, i), s = i.children.length - 1; s >= 0; s--) {
                        var a = i.children[s],
                                l = n(r, a);
                        null == l && o.children.splice(s, 1)
                    }
                    return o.children.length > 0 ? o : n(r, o)
                }
                var c = t(i.text).toUpperCase(),
                        u = t(r.term).toUpperCase();
                return c.indexOf(u) > -1 ? i : null
            }
            this.defaults = {
                amdBase: "select2/",
                amdLanguageBase: "select2/i18n/",
                language: E,
                matcher: n,
                minimumInputLength: 0,
                maximumInputLength: 0,
                maximumSelectionLength: 0,
                minimumResultsForSearch: 0,
                selectOnClose: !1,
                sorter: function (e) {
                    return e
                },
                templateResult: function (e) {
                    return e.text
                },
                templateSelection: function (e) {
                    return e.text
                },
                theme: "default",
                width: "resolve"
            }
        }, C.prototype.set = function (t, n) {
            var r = e.camelCase(t),
                    i = {};
            i[r] = n;
            var o = c._convertData(i);
            e.extend(this.defaults, o)
        };
        var D = new C;
        return D
    }), e("select2/options", ["jquery", "./defaults", "./utils"], function (e, t, n) {
        function r(e, n) {
            this.options = e, null != n && this.fromElement(n), this.options = t.apply(this.options)
        }
        return r.prototype.fromElement = function (t) {
            var r = ["select2"];
            null == this.options.multiple && (this.options.multiple = t.prop("multiple")), null == this.options.disabled && (this.options.disabled = t.prop("disabled")), null == this.options.language && (t.prop("lang") ? this.options.language = t.prop("lang").toLowerCase() : t.closest("[lang]").prop("lang") && (this.options.language = t.closest("[lang]").prop("lang"))), null == this.options.dir && (this.options.dir = t.prop("dir") ? t.prop("dir") : t.closest("[dir]").prop("dir") ? t.closest("[dir]").prop("dir") : "ltr"), t.prop("disabled", this.options.disabled), t.prop("multiple", this.options.multiple), t.data("select2-tags") && (console && console.warn && console.warn('Select2: The `data-select2-tags` attribute has been changed to use the `data-data` and `data-tags="true"` attributes and will be removed in future versions of Select2.'), t.data("data", t.data("select2-tags")), t.data("tags", !0)), t.data("ajax-url") && (console && console.warn && console.warn("Select2: The `data-ajax-url` attribute has been changed to `data-ajax--url` and support for the old attribute will be removed in future versions of Select2."), t.data("ajax--url", t.data("ajax-url")));
            var i = t.data();
            i = n._convertData(i);
            for (var o in i) r.indexOf(o) > -1 || (e.isPlainObject(this.options[o]) ? e.extend(this.options[o], i[o]) : this.options[o] = i[o]);
            return this
        }, r.prototype.get = function (e) {
            return this.options[e]
        }, r.prototype.set = function (e, t) {
            this.options[e] = t
        }, r
    }), e("select2/core", ["jquery", "./options", "./utils", "./keys"], function (e, t, n, r) {
        var o = function (e, n) {
            null != e.data("select2") && e.data("select2").destroy(), this.$element = e, this.id = this._generateId(e), n = n || {}, this.options = new t(n, e), o.__super__.constructor.call(this);
            var r = this.options.get("dataAdapter");
            this.data = new r(e, this.options);
            var i = this.render();
            this._placeContainer(i);
            var s = this.options.get("selectionAdapter");
            this.selection = new s(e, this.options), this.$selection = this.selection.render(), this.selection.position(this.$selection, i);
            var a = this.options.get("dropdownAdapter");
            this.dropdown = new a(e, this.options), this.$dropdown = this.dropdown.render(), this.dropdown.position(this.$dropdown, i);
            var l = this.options.get("resultsAdapter");
            this.results = new l(e, this.options, this.data), this.$results = this.results.render(), this.results.position(this.$results, this.$dropdown);
            var c = this;
            this._bindAdapters(), this._registerDomEvents(), this._registerDataEvents(), this._registerSelectionEvents(), this._registerDropdownEvents(), this._registerResultsEvents(), this._registerEvents(), this.data.current(function (e) {
                c.trigger("selection:update", {
                    data: e
                })
            }), e.hide(), this._syncAttributes(), this._tabindex = e.attr("tabindex") || 0, e.attr("tabindex", "-1"), e.data("select2", this)
        };
        return n.Extend(o, n.Observable), o.prototype._generateId = function (e) {
            var t = "";
            return t = null != e.attr("id") ? e.attr("id") : null != e.attr("name") ? e.attr("name") + "-" + n.generateChars(2) : n.generateChars(4), t = "select2-" + t
        }, o.prototype._placeContainer = function (e) {
            e.insertAfter(this.$element);
            var t = this._resolveWidth(this.$element, this.options.get("width"));
            null != t && e.css("width", t)
        }, o.prototype._resolveWidth = function (e, t) {
            var n = /^width:(([-+]?([0-9]*\.)?[0-9]+)(px|em|ex|%|in|cm|mm|pt|pc))/i;
            if ("resolve" == t) {
                var r = this._resolveWidth(e, "style");
                return null != r ? r : this._resolveWidth(e, "element")
            }
            if ("element" == t) {
                var o = e.outerWidth(!1);
                return 0 >= o ? "auto" : o + "px"
            }
            if ("style" == t) {
                var s = e.attr("style");
                if ("string" != typeof s) return null;
                var a = s.split(";");
                for (i = 0, l = a.length; i < l; i += 1) {
                    attr = a[i].replace(/\s/g, "");
                    var c = attr.match(n);
                    if (null !== c && c.length >= 1) return c[1]
                }
                return null
            }
            return t
        }, o.prototype._bindAdapters = function () {
            this.data.bind(this, this.$container), this.selection.bind(this, this.$container), this.dropdown.bind(this, this.$container), this.results.bind(this, this.$container)
        }, o.prototype._registerDomEvents = function () {
            var t = this;
            this.$element.on("change.select2", function () {
                t.data.current(function (e) {
                    t.trigger("selection:update", {
                        data: e
                    })
                })
            }), this._sync = n.bind(this._syncAttributes, this), this.$element[0].attachEvent && this.$element[0].attachEvent("onpropertychange", this._sync);
            var r = window.MutationObserver || window.WebKitMutationObserver || window.MozMutationObserver;
            null != r && (this._observer = new r(function (n) {
                e.each(n, t._sync)
            }), this._observer.observe(this.$element[0], {
                attributes: !0,
                subtree: !1
            }))
        }, o.prototype._registerDataEvents = function () {
            var e = this;
            this.data.on("*", function (t, n) {
                e.trigger(t, n)
            })
        }, o.prototype._registerSelectionEvents = function () {
            var e = this,
                    t = ["toggle"];
            this.selection.on("toggle", function () {
                e.toggleDropdown()
            }), this.selection.on("*", function (n, r) {
-1 === t.indexOf(n) && e.trigger(n, r)
            })
        }, o.prototype._registerDropdownEvents = function () {
            var e = this;
            this.dropdown.on("*", function (t, n) {
                e.trigger(t, n)
            })
        }, o.prototype._registerResultsEvents = function () {
            var e = this;
            this.results.on("*", function (t, n) {
                e.trigger(t, n)
            })
        }, o.prototype._registerEvents = function () {
            var e = this;
            this.on("open", function () {
                e.$container.addClass("select2-container--open")
            }), this.on("close", function () {
                e.$container.removeClass("select2-container--open")
            }), this.on("enable", function () {
                e.$container.removeClass("select2-container--disabled")
            }), this.on("disable", function () {
                e.$container.addClass("select2-container--disabled")
            }), this.on("query", function (t) {
                this.data.query(t, function (n) {
                    e.trigger("results:all", {
                        data: n,
                        query: t
                    })
                })
            }), this.on("query:append", function (t) {
                this.data.query(t, function (n) {
                    e.trigger("results:append", {
                        data: n,
                        query: t
                    })
                })
            }), this.on("keypress", function (t) {
                var n = t.which;
                e.isOpen() ? n === r.ENTER ? (e.trigger("results:select"), t.preventDefault()) : n === r.UP ? (e.trigger("results:previous"), t.preventDefault()) : n === r.DOWN ? (e.trigger("results:next"), t.preventDefault()) : (n === r.ESC || n === r.TAB) && (e.close(), t.preventDefault()) : (n === r.ENTER || n === r.SPACE || (n === r.DOWN || n === r.UP) && t.altKey) && (e.open(), t.preventDefault())
            })
        }, o.prototype._syncAttributes = function () {
            this.options.set("disabled", this.$element.prop("disabled")), this.options.get("disabled") ? (this.isOpen() && this.close(), this.trigger("disable")) : this.trigger("enable")
        }, o.prototype.trigger = function (e, t) {
            var n = o.__super__.trigger,
                    r = {
                        open: "opening",
                        close: "closing",
                        select: "selecting",
                        unselect: "unselecting"
                    };
            if (e in r) {
                var i = r[e],
                        s = {
                            prevented: !1,
                            name: e,
                            args: t
                        };
                if (n.call(this, i, s), s.prevented) return void (t.prevented = !0)
            }
            n.call(this, e, t)
        }, o.prototype.toggleDropdown = function () {
            this.options.get("disabled") || (this.isOpen() ? this.close() : this.open())
        }, o.prototype.open = function () {
            this.isOpen() || (this.trigger("query", {}), this.trigger("open"))
        }, o.prototype.close = function () {
            this.isOpen() && this.trigger("close")
        }, o.prototype.isOpen = function () {
            return this.$container.hasClass("select2-container--open")
        }, o.prototype.enable = function (e) {
            console && console.warn && console.warn('Select2: The `select2("enable")` method has been deprecated and will be removed in later Select2 versions. Use $element.prop("disabled") instead.'), 0 === e.length && (e = [!0]);
            var t = !e[0];
            this.$element.prop("disabled", t)
        }, o.prototype.val = function (t) {
            if (console && console.warn && console.warn('Select2: The `select2("val")` method has been deprecated and will be removed in later Select2 versions. Use $element.val() instead.'), 0 === t.length) return this.$element.val();
            var n = t[0];
            e.isArray(n) && (n = e.map(n, function (e) {
                return e.toString()
            })), this.$element.val(n).trigger("change")
        }, o.prototype.destroy = function () {
            this.$container.remove(), this.$element[0].detachEvent && this.$element[0].detachEvent("onpropertychange", this._sync), null != this._observer && (this._observer.disconnect(), this._observer = null), this._sync = null, this.$element.off(".select2"), this.$element.attr("tabindex", this._tabindex), this.$element.show(), this.$element.removeData("select2"), this.data.destroy(), this.selection.destroy(), this.dropdown.destroy(), this.results.destroy(), this.data = null, this.selection = null, this.dropdown = null, this.results = null
        }, o.prototype.render = function () {
            var t = e('<span class="select2 select2-container"><span class="selection"></span><span class="dropdown-wrapper" aria-hidden="true"></span></span>');
            return t.attr("dir", this.options.get("dir")), this.$container = t, this.$container.addClass("select2-container--" + this.options.get("theme")), t.data("element", this.$element), t
        }, o
    }),
        function (t) {
            "function" == typeof e && e.amd ? e("jquery.mousewheel", ["jquery"], t) : "object" == typeof exports ? module.exports = t : t(jQuery)
        } (function (e) {
            function t(t) {
                var s = t || window.event,
                    a = l.call(arguments, 1),
                    c = 0,
                    d = 0,
                    h = 0,
                    p = 0,
                    f = 0,
                    g = 0;
                if (t = e.event.fix(s), t.type = "mousewheel", "detail" in s && (h = -1 * s.detail), "wheelDelta" in s && (h = s.wheelDelta), "wheelDeltaY" in s && (h = s.wheelDeltaY), "wheelDeltaX" in s && (d = -1 * s.wheelDeltaX), "axis" in s && s.axis === s.HORIZONTAL_AXIS && (d = -1 * h, h = 0), c = 0 === h ? d : h, "deltaY" in s && (h = -1 * s.deltaY, c = h), "deltaX" in s && (d = s.deltaX, 0 === h && (c = -1 * d)), 0 !== h || 0 !== d) {
                    if (1 === s.deltaMode) {
                        var m = e.data(this, "mousewheel-line-height");
                        c *= m, h *= m, d *= m
                    } else if (2 === s.deltaMode) {
                        var v = e.data(this, "mousewheel-page-height");
                        c *= v, h *= v, d *= v
                    }
                    if (p = Math.max(Math.abs(h), Math.abs(d)), (!o || o > p) && (o = p, r(s, p) && (o /= 40)), r(s, p) && (c /= 40, d /= 40, h /= 40), c = Math[c >= 1 ? "floor" : "ceil"](c / o), d = Math[d >= 1 ? "floor" : "ceil"](d / o), h = Math[h >= 1 ? "floor" : "ceil"](h / o), u.settings.normalizeOffset && this.getBoundingClientRect) {
                        var y = this.getBoundingClientRect();
                        f = t.clientX - y.left, g = t.clientY - y.top
                    }
                    return t.deltaX = d, t.deltaY = h, t.deltaFactor = o, t.offsetX = f, t.offsetY = g, t.deltaMode = 0, a.unshift(t, c, d, h), i && clearTimeout(i), i = setTimeout(n, 200), (e.event.dispatch || e.event.handle).apply(this, a)
                }
            }

            function n() {
                o = null
            }

            function r(e, t) {
                return u.settings.adjustOldDeltas && "mousewheel" === e.type && t % 120 === 0
            }
            var i, o, s = ["wheel", "mousewheel", "DOMMouseScroll", "MozMousePixelScroll"],
                a = "onwheel" in document || document.documentMode >= 9 ? ["wheel"] : ["mousewheel", "DomMouseScroll", "MozMousePixelScroll"],
                l = Array.prototype.slice;
            if (e.event.fixHooks)
                for (var c = s.length; c; ) e.event.fixHooks[s[--c]] = e.event.mouseHooks;
            var u = e.event.special.mousewheel = {
                version: "3.1.12",
                setup: function () {
                    if (this.addEventListener)
                        for (var n = a.length; n; ) this.addEventListener(a[--n], t, !1);
                    else this.onmousewheel = t;
                    e.data(this, "mousewheel-line-height", u.getLineHeight(this)), e.data(this, "mousewheel-page-height", u.getPageHeight(this))
                },
                teardown: function () {
                    if (this.removeEventListener)
                        for (var n = a.length; n; ) this.removeEventListener(a[--n], t, !1);
                    else this.onmousewheel = null;
                    e.removeData(this, "mousewheel-line-height"), e.removeData(this, "mousewheel-page-height")
                },
                getLineHeight: function (t) {
                    var n = e(t),
                        r = n["offsetParent" in e.fn ? "offsetParent" : "parent"]();
                    return r.length || (r = e("body")), parseInt(r.css("fontSize"), 10) || parseInt(n.css("fontSize"), 10) || 16
                },
                getPageHeight: function (t) {
                    return e(t).height()
                },
                settings: {
                    adjustOldDeltas: !0,
                    normalizeOffset: !0
                }
            };
            e.fn.extend({
                mousewheel: function (e) {
                    return e ? this.bind("mousewheel", e) : this.trigger("mousewheel")
                },
                unmousewheel: function (e) {
                    return this.unbind("mousewheel", e)
                }
            })
        }), e("select2/compat/matcher", ["jquery"], function (e) {
            function t(t) {
                function n(n, r) {
                    var i = e.extend(!0, {}, r);
                    if (null == n.term || "" === e.trim(n.term)) return i;
                    if (r.children) {
                        for (var o = r.children.length - 1; o >= 0; o--) {
                            var s = r.children[o],
                                a = t(n.term, s.text, s);
                            a || i.children.splice(o, 1)
                        }
                        if (i.children.length > 0) return i
                    }
                    return t(n.term, r.text, r) ? i : null
                }
                return n
            }
            return t
        }), e("select2/compat/initSelection", ["jquery"], function (e) {
            function t(e, t, n) {
                console && console.warn && console.warn("Select2: The `initSelection` option has been deprecated in favor of a custom data adapter that overrides the `current` method. This method is now called multiple times instead of a single time when the instance is initialized. Support will be removed for the `initSelection` option in future versions of Select2"), this.initSelection = n.get("initSelection"), this._isInitialized = !1, e.call(this, t, n)
            }
            return t.prototype.current = function (t, n) {
                var r = this;
                return this._isInitialized ? void t.call(this, n) : void this.initSelection.call(null, this.$element, function (t) {
                    r._isInitialized = !0, e.isArray(t) || (t = [t]), n(t)
                })
            }, t
        }), e("select2/compat/query", [], function () {
            function e(e, t, n) {
                console && console.warn && console.warn("Select2: The `query` option has been deprecated in favor of a custom data adapter that overrides the `query` method. Support will be removed for the `query` option in future versions of Select2."), e.call(this, t, n)
            }
            return e.prototype.query = function (e, t, n) {
                t.callback = n;
                var r = this.options.get("query");
                r.call(null, t)
            }, e
        }), e("select2/dropdown/attachContainer", [], function () {
            function e(e, t, n) {
                e.call(this, t, n)
            }
            return e.prototype.position = function (e, t, n) {
                var r = n.find(".dropdown-wrapper");
                r.append(t), t.addClass("select2-dropdown--below"), n.addClass("select2-container--below")
            }, e
        }), e("jquery.select2", ["jquery", "./select2/core", "./select2/defaults"], function (e, n, r) {
            try {
                t("jquery.mousewheel")
            } catch (i) { }
            return null == e.fn.select2 && (e.fn.select2 = function (t) {
                if (t = t || {}, "object" == typeof t) return this.each(function () {
                    {
                        var r = e.extend({}, t, !0);
                        new n(e(this), r)
                    }
                }), this;
                if ("string" == typeof t) {
                    var r = this.data("select2"),
                        i = Array.prototype.slice.call(arguments, 1);
                    return r[t](i)
                }
                throw new Error("Invalid arguments for Select2: " + t)
            }), null == e.fn.select2.defaults && (e.fn.select2.defaults = r), n
        }), t("jquery.select2"), jQuery.fn.select2.amd = {
            define: e,
            require: t
        }
} ();