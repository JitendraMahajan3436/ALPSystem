
RGraph = window.RGraph || { isRGraph: true }; RGraph.SemiCircularProgress = function (conf) {
    if (typeof conf === 'object' && typeof conf.id === 'string') { var parseConfObjectForOptions = true; } else { var conf = { id: arguments[0], min: arguments[1], max: arguments[2], value: arguments[3] } }
    this.id = conf.id; this.canvas = document.getElementById(this.id); this.context = this.canvas.getContext('2d'); this.canvas.__object__ = this; this.min = RGraph.stringsToNumbers(conf.min); this.max = RGraph.stringsToNumbers(conf.max); this.value = RGraph.stringsToNumbers(conf.value); this.type = 'semicircularprogress'; this.coords = []; this.isRGraph = true; this.currentValue = null; this.uid = RGraph.createUID(); this.canvas.uid = this.canvas.uid ? this.canvas.uid : RGraph.CreateUID(); this.colorsParsed = false; this.coordsText = []; this.original_colors = []; this.firstDraw = true; this.properties = { 'chart.background.color': 'rgba(0,0,0,0)', 'chart.colors': ['#0c0'], 'chart.linewidth': 2, 'chart.strokestyle': '#666', 'chart.gutter.left': 25, 'chart.gutter.right': 25, 'chart.gutter.top': 25, 'chart.gutter.bottom': 35, 'chart.radius': null, 'chart.centerx': null, 'chart.centery': null, 'chart.width': null, 'chart.angles.start': Math.PI, 'chart.angles.end': (2 * Math.PI), 'chart.scale.decimals': 2, 'chart.scale.point': '.', 'chart.scale.thousand': ',', 'chart.scale.formatter': null, 'chart.scale.round': false, 'chart.shadow': false, 'chart.shadow.color': 'rgba(220,220,220,1)', 'chart.shadow.blur': 2, 'chart.shadow.offsetx': 2, 'chart.shadow.offsety': 2, 'chart.labels.center': true, 'chart.labels.center.font': null, 'chart.labels.center.bold': false, 'chart.labels.center.italic': false, 'chart.labels.center.fade': false, 'chart.labels.center.size': 40, 'chart.labels.center.color': 'black', 'chart.labels.center.valign': 'bottom', 'chart.labels.min.color': null, 'chart.labels.min.font': null, 'chart.labels.min.bold': false, 'chart.labels.min.size': null, 'chart.labels.min.italic': false, 'chart.labels.min.offset.angle': 0, 'chart.labels.min.offsetx': 0, 'chart.labels.min.offsety': 0, 'chart.labels.max.color': null, 'chart.labels.max.font': null, 'chart.labels.max.bold': false, 'chart.labels.max.size': null, 'chart.labels.max.italic': false, 'chart.labels.max.offset.angle': 0, 'chart.labels.max.offsetx': 0, 'chart.labels.max.offsety': 0, 'chart.title': '', 'chart.title.bold': true, 'chart.title.italic': false, 'chart.title.font': null, 'chart.title.size': null, 'chart.title.color': 'black', 'chart.text.size': 12, 'chart.text.color': 'black', 'chart.text.font': 'Segoe UI, Arial, Verdana, sans-serif', 'chart.text.accessible': true, 'chart.text.accessible.overflow': 'visible', 'chart.text.accessible.pointerevents': true, 'chart.contextmenu': null,'chart.center.units.post':'', 'chart.units.pre': '', 'chart.units.post': '', 'chart.tooltips': null, 'chart.tooltips.effect': 'fade', 'chart.tooltips.css.class': 'RGraph_tooltip', 'chart.tooltips.highlight': true, 'chart.tooltips.event': 'onclick', 'chart.tooltips.coords.page': true, 'chart.highlight.stroke': 'rgba(0,0,0,0)', 'chart.highlight.fill': 'rgba(255,255,255,0.7)', 'chart.annotatable': false, 'chart.annotate.color': 'black', 'chart.zoom.factor': 1.5, 'chart.zoom.fade.in': true, 'chart.zoom.fade.out': true, 'chart.zoom.hdir': 'right', 'chart.zoom.vdir': 'down', 'chart.zoom.frames': 25, 'chart.zoom.delay': 16.666, 'chart.zoom.shadow': true, 'chart.zoom.background': true, 'chart.zoom.action': 'zoom', 'chart.resizable': false, 'chart.resize.handle.adjust': [0, 0], 'chart.resize.handle.background': null, 'chart.adjustable': false, 'chart.events.click': null, 'chart.events.mousemove': null, 'chart.clearto': 'rgba(0,0,0,0)' }
    if (!this.canvas) { alert('[SEMICIRCULARPROGRESS] No canvas support'); return; }
    if (!this.canvas.__rgraph_aa_translated__) { this.context.translate(0.5, 0.5); this.canvas.__rgraph_aa_translated__ = true; }
    var RG = RGraph, ca = this.canvas, co = ca.getContext('2d'), prop = this.properties, pa2 = RG.path2, win = window, doc = document, ma = Math
    if (RG.Effects && typeof RG.Effects.decorate === 'function') { RG.Effects.decorate(this); }
    this.set = this.Set = function (name) {
        var value = typeof arguments[1] === 'undefined' ? null : arguments[1]; if (arguments.length === 1 && typeof name === 'object') { RG.parseObjectStyleConfig(this, name); return this; }
        if (name.substr(0, 6) != 'chart.') { name = 'chart.' + name; }
        while (name.match(/([A-Z])/)) { name = name.replace(/([A-Z])/, '.' + RegExp.$1.toLowerCase()); }
        prop[name.toLowerCase()] = value; return this;
    }; this.get = this.Get = function (name) {
        if (name.substr(0, 6) != 'chart.') { name = 'chart.' + name; }
        while (name.match(/([A-Z])/)) { name = name.replace(/([A-Z])/, '.' + RegExp.$1.toLowerCase()); }
        return prop[name.toLowerCase()];
    }; this.draw = this.Draw = function () {
        RG.fireCustomEvent(this, 'onbeforedraw'); if (!this.colorsParsed) { this.parseColors(); this.colorsParsed = true; }
        this.currentValue = this.value; this.gutterLeft = prop['chart.gutter.left']; this.gutterRight = prop['chart.gutter.right']; this.gutterTop = prop['chart.gutter.top']; this.gutterBottom = prop['chart.gutter.bottom']; this.radius = ma.min((ca.width - prop['chart.gutter.left'] - prop['chart.gutter.right']) / 2, ca.height - prop['chart.gutter.top'] - prop['chart.gutter.bottom']); this.centerx = ((ca.width - this.gutterLeft - this.gutterRight) / 2) + this.gutterLeft; this.centery = ca.height - this.gutterBottom; this.width = this.radius / 3; if (typeof prop['chart.radius'] === 'number') this.radius = prop['chart.radius']; if (typeof prop['chart.centerx'] === 'number') this.centerx = prop['chart.centerx']; if (typeof prop['chart.centery'] === 'number') this.centery = prop['chart.centery']; if (typeof prop['chart.width'] === 'number') this.width = prop['chart.width']; this.coords = []; this.coordsText = []; this.drawMeter(); this.drawLabels(); if (prop['chart.contextmenu']) { RG.showContext(this); }
        RG.installEventListeners(this); if (prop['chart.resizable']) { RG.allowResizing(this); }
        this.allowAdjusting(); if (this.firstDraw) { this.firstDraw = false; RG.fireCustomEvent(this, 'onfirstdraw'); this.firstDrawFunc(); }
        RG.fireCustomEvent(this, 'ondraw'); return this;
    }; this.drawMeter = this.DrawMeter = function () {
        var start = prop['chart.angles.start'], end = prop['chart.angles.end']; this.scale2 = RG.getScale2(this, { 'max': this.max, 'strict': true, 'min': this.min, 'scale.thousand': prop['chart.scale.thousand'], 'scale.point': prop['chart.scale.point'], 'scale.decimals': prop['chart.scale.decimals'], 'ylabels.count': 5, 'units.pre': prop['chart.units.pre'], 'units.post': prop['chart.units.post'] }); if (prop['chart.background.color'] !== 'rgba(0,0,0,0)') { pa2(co, 'fs % fr % % % %', prop['chart.background.color'], 0, 0, ca.width, ca.height); }
        pa2(co, 'lw % b a % % % % % false a % % % % % true c s % f % sx % sy % sc % sb % f % sx 0 sy 0 sb 0 sc rgba(0,0,0,0) lw 1', prop['chart.linewidth'], this.centerx, this.centery, this.radius, start, end, this.centerx, this.centery, this.radius - this.width, end, start, prop['chart.strokestyle'], typeof prop['chart.colors'][1] !== 'undefined' ? prop['chart.colors'][1] : prop['chart.colors'][0], prop['chart.shadow.offsetx'], prop['chart.shadow.offsety'], prop['chart.shadow'] ? prop['chart.shadow.color'] : 'rgba(0,0,0,0)', prop['chart.shadow.blur'], typeof prop['chart.colors'][1] !== 'undefined' ? 'rgba(0,0,0,0)' : 'rgba(255,255,255,0.85)'); var angle = start + ((end - start) * ((this.value - this.scale2.min) / (this.max - this.scale2.min))); pa2(co, 'b a % % % % % false a % % % % % true c f %', this.centerx, this.centery, this.radius, start, angle, this.centerx, this.centery, this.radius - this.width, start + ((end - start) * ((this.value - this.scale2.min) / (this.max - this.scale2.min))), start, prop['chart.colors'][0]); this.coords = [[this.centerx, this.centery, this.radius, start, end, this.width, angle]];
    }; this.drawLabels = this.DrawLabels = function () {
        var min = RG.numberFormat(this, this.scale2.min)
        var max = RG.numberFormat(this, this.scale2.max)
        if (prop['chart.angles.start'] === RGraph.PI) { var halign = 'center'; var valign = 'top'; } else if (prop['chart.angles.start'] <= RGraph.PI) { var halign = 'left'; var valign = 'center'; } else if (prop['chart.angles.start'] >= RGraph.PI) { var halign = 'right'; var valign = 'center'; }
        var xy = RG.getRadiusEndPoint(this.centerx, this.centery, prop['chart.angles.start'] + prop['chart.labels.min.offset.angle'], this.radius - (this.width / 2)); RG.text2(this, { font: prop['chart.labels.min.font'] || prop['chart.text.font'], bold: prop['chart.labels.min.bold'] || prop['chart.text.bold'], size: prop['chart.labels.min.size'] || prop['chart.text.size'], x: xy[0] + prop['chart.labels.min.offsetx'], y: xy[1] + prop['chart.labels.min.offsety'], valign: valign, halign: halign, text: min, color: prop['chart.labels.min.color'] || prop['chart.text.color'], italic: prop['chart.labels.min.italic'] }); if (prop['chart.angles.end'] === RGraph.TWOPI) { var halign = 'center'; var valign = 'top'; } else if (prop['chart.angles.end'] >= RGraph.TWOPI) { var halign = 'right'; var valign = 'center'; } else if (prop['chart.angles.end'] <= RGraph.TWOPI) { var halign = 'left'; var valign = 'center'; }
        var xy = RG.getRadiusEndPoint(this.centerx, this.centery, prop['chart.angles.end'] + prop['chart.labels.max.offset.angle'], this.radius - (this.width / 2)); RG.text2(this, { font: prop['chart.labels.max.font'] || prop['chart.text.font'], bold: prop['chart.labels.max.bold'] || prop['chart.text.bold'], size: prop['chart.labels.max.size'] || prop['chart.text.size'], x: xy[0] + prop['chart.labels.max.offsetx'], y: xy[1] + prop['chart.labels.max.offsety'], valign: valign, halign: halign, text: max, color: prop['chart.labels.max.color'] || prop['chart.text.color'], italic: prop['chart.labels.max.italic'] }); if (prop['chart.labels.center']) {
            var ret = RG.text2(this, { font: prop['chart.labels.center.font'] || prop['chart.text.font'], size: prop['chart.labels.center.size'] || 50, bold: prop['chart.labels.center.bold'], italic: prop['chart.labels.center.italic'], x: this.centerx , y: this.centery, valign: prop['chart.labels.center.valign'], halign: 'center', text: RG.numberFormat(this, this.value.toFixed(prop['chart.scale.decimals']) , prop['chart.units.pre'], prop['chart.units.post']), color: prop['chart.labels.center.color'] || prop['chart.text.color'] }); if (prop['chart.labels.center.fade'] && ret.node) {
                ret.node.style.opacity = 0; var delay = 25, incr = 0.1; for (var i = 0; i < 10; ++i) {
                    (function (index) {
                        setTimeout(function ()
                        { ret.node.style.opacity = incr * index; }, delay * (index + 1));
                    })(i);
                }
            }
        }
        RG.drawTitle(this, prop['chart.title'], this.gutterTop, null, prop['chart.title.size']);
    }; this.getShape = function (e) {
        var mouseXY = RG.getMouseXY(e), mouseX = mouseXY[0], mouseY = mouseXY[1]
        pa2(co, 'b a % % % % % false a % % % % % true', this.coords[0][0], this.coords[0][1], this.coords[0][2], this.coords[0][3], this.coords[0][6], this.coords[0][0], this.coords[0][1], this.coords[0][2] - this.coords[0][5], this.coords[0][6], this.coords[0][3]); if (co.isPointInPath(mouseX, mouseY)) { return { object: this, 0: this, x: this.coords[0][0], 1: this.coords[0][0], y: this.coords[0][1], 2: this.coords[0][1], radius: this.coords[0][2], 3: this.coords[0][2], width: this.coords[0][5], 4: this.coords[0][5], start: this.coords[0][3], 5: this.coords[0][3], end: this.coords[0][6], 6: this.coords[0][6], index: 0, tooltip: !RG.isNull(prop['chart.tooltips']) ? prop['chart.tooltips'][0] : null }; }
    }; this.getValue = function (e) {
        var mouseXY = RG.getMouseXY(e), mouseX = mouseXY[0], mouseY = mouseXY[1], angle = RG.getAngleByXY(this.centerx, this.centery, mouseX, mouseY); if (angle && mouseX >= this.centerx && mouseY > this.centery) { angle += RGraph.TWOPI; }
        if (angle < prop['chart.angles.start'] && mouseX > this.centerx) { angle = prop['chart.angles.end']; }
        if (angle < prop['chart.angles.start']) { angle = prop['chart.angles.start']; }
        var value = (((angle - prop['chart.angles.start']) / (prop['chart.angles.end'] - prop['chart.angles.start'])) * (this.max - this.min)) + this.min;
        value = ma.max(value, this.min);
        value = ma.min(value, this.max);
        return value;
    }; this.highlight = this.Highlight = function (shape)
    { if (typeof prop['chart.highlight.style'] === 'function') { (prop['chart.highlight.style'])(shape); } else { pa2(co, 'lw 5 b a % % % % % false a % % % % % true c s % f % lw 1', shape.x, shape.y, shape.radius, shape.start, shape.end, shape.x, shape.y, shape.radius - shape.width, shape.end, shape.start, prop['chart.highlight.stroke'], prop['chart.highlight.fill']); } }; this.getObjectByXY = function (e)
    { var mouseXY = RG.getMouseXY(e); pa2(co, 'b a % % % % % false', this.centerx, this.centery, this.radius, prop['chart.angles.start'], prop['chart.angles.end']); pa2(co, 'a % % % % % true', this.centerx, this.centery, this.radius - this.width, prop['chart.angles.end'], prop['chart.angles.start']); return co.isPointInPath(mouseXY[0], mouseXY[1]) ? this : null; }; this.allowAdjusting = this.AllowAdjusting = function () { }; this.adjusting_mousemove = this.Adjusting_mousemove = function (e)
    { if (prop['chart.adjustable'] && RG.Registry.Get('chart.adjusting') && RG.Registry.Get('chart.adjusting').uid == this.uid) { var value = this.getValue(e); if (typeof value === 'number') { RG.fireCustomEvent(this, 'onadjust'); this.value = Number(value.toFixed(prop['chart.scale.decimals'])); RG.redrawCanvas(this.canvas); } } }; this.getAngle = function (value) {
        if (value > this.max || value < this.min) { return null; }
        var angle = (value / this.max) * (prop['chart.angles.end'] - prop['chart.angles.start'])
        angle += prop['chart.angles.start']; return angle;
    }; this.overChartArea = function (e) {
        var mouseXY = RGraph.getMouseXY(e), mouseX = mouseXY[0], mouseY = mouseXY[1]
        pa2(co, 'b a % % % % % false a % % % % % true', this.coords[0][0], this.coords[0][1], this.coords[0][2], prop['chart.angles.start'], prop['chart.angles.end'], this.coords[0][0], this.coords[0][1], this.coords[0][2] - this.coords[0][5], prop['chart.angles.end'], prop['chart.angles.start']); return co.isPointInPath(mouseX, mouseY);
    };
    this.parseColors = function () {
        if (this.original_colors.length === 0) { this.original_colors['chart.colors'] = RG.arrayClone(prop['chart.colors']); }
        prop['chart.colors'][0] = this.parseSingleColorForGradient(prop['chart.colors'][0]); prop['chart.colors'][1] = this.parseSingleColorForGradient(prop['chart.colors'][1]); prop['chart.strokestyle'] = this.parseSingleColorForGradient(prop['chart.strokestyle']); prop['chart.background.color'] = this.parseSingleColorForGradient(prop['chart.background.color']);
    };
    this.reset = function ()
    { };
    this.parseSingleColorForGradient = function (color) {
        if (!color || typeof color != 'string') { return color; }
        if (color.match(/^gradient\((.*)\)$/i)) {
            var parts = RegExp.$1.split(':'); var grad = co.createLinearGradient(prop['chart.gutter.left'], 0, ca.width - prop['chart.gutter.right'], 0); var diff = 1 / (parts.length - 1); grad.addColorStop(0, RG.trim(parts[0])); for (var j = 1, len = parts.length; j < len; ++j) { grad.addColorStop(j * diff, RG.trim(parts[j])); }
            return grad ? grad : 'White';
        }
        return grad ? grad : color;
    }; this.on = function (type, func) {
        if (type.substr(0, 2) !== 'on') { type = 'on' + type; }
        if (typeof this[type] !== 'function') { this[type] = func; } else { RG.addCustomEventListener(this, type, func); }
        return this;
    }; this.exec = function (func)
    { func(this); return this; }; this.firstDrawFunc = function ()
    { }; this.grow = function () {
        
        var obj = this, initial_value = this.currentValue, opt = arguments[0] || {}, numFrames = opt.frames || 30, frame = 0, callback = arguments[1] || function () { }, diff = this.value - this.currentValue, increment = diff / numFrames
        function iterator()
        { frame++; if (frame <= numFrames) { obj.value = initial_value + (increment * frame); RG.clear(ca); RG.redrawCanvas(ca); RG.Effects.updateCanvas(iterator); } else { callback(); } }
        iterator(); return this;
    }; RG.Register(this); if (parseConfObjectForOptions) { RG.parseObjectStyleConfig(this, conf.options); }
};