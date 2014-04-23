/// <reference path="libs/jquery.min.js" />
var modelos = {};
var vistas = {};
var App = null;

window.Handlebars.registerHelper('select', function (value, options) {
    var $el = $('<select />').html(options.fn(this));
    $el.find('[value=' + value + ']').attr({ 'selected': 'selected' });
    return $el.html();
});

$(document).ready(function () {
    createCalendar();
    startBackbone();
});

function createCalendar() {
    $("#calendar").fullCalendar({
        lang: 'es',
        header: {
            left: "prev,next,today",
            center: "title",
            right: "month,basicWeek,basicDay"
        },
        eventDurationEditable:false,
        events:currentUrl+"actividades",
        defaultDate: moment().format('YYYY-MM-DD'),
        editable: true,
        timezone:"local",
        droppable: true,
        drop: function(moment, allDay) {
            console.log($(this).data("idproy"));
            var tmpAct = new modelos.Actividad({
                IdProyecto: $(this).data("idproy"),
                Fecha:moment
            });
            tmpAct.save(null, {
                success: function (model, response) {
                    //var eventObject = { id: model.get("id"), title: model.get("nombre") };
                    //eventObject.start = moment;
                    //eventObject.allDay = allDay;
                    //$('#calendar').fullCalendar('renderEvent', eventObject, true);
                    $('#calendar').fullCalendar('refetchEvents');
                },
                error: function (model, response) {
                    console.log("error");
                }
            });
           
        },
        eventRender: function (event, element) {
            $(element).attr("id", "event" + event.id);
            $(element).popover({
                placement: "top",
                title: "Comentario",
                html:true,
                container: "#popups",
                content: "<textarea class='comment' cols='4' rows='3'>"+event.comment+"</textarea><button class='btn btn-primary saveComment' onclick='saveComment(this," + event.id + ")'>Guardar</button>",
            });
            element.prepend("<a class='close' href='#' onclick='removeEvent(" + event.id + ");'>&times;</a>");
        },
    });

}

function saveComment(self, eventId) {
    var comment = $(self.parentElement).find(".comment").val();
    if (comment.length > 0) {
        $.post(currentUrl+"comentario",{id:eventId,comment:comment});
    }
    $("#event"+eventId).popover("hide");
}

function startBackbone() {
    modelos.Actividad = Backbone.Model.extend({
        urlRoot: currentUrl +"actividad",
        //url: currentUrl + "actividad",
        idAttribute: "id",
    });
    modelos.Proyecto = Backbone.Model.extend({
        urlRoot: currentUrl + "proyectos",
        idAttribute:"id",
    });
    modelos.ProyectosCollection = Backbone.Collection.extend({
        urlRoot: currentUrl + "proyectos",
        url: currentUrl + "proyectos",
        model:modelos.Proyecto
    });

   
    //-------------------Inician vistas backbone------------------------------------------
    vistas.ProyectosListView = Backbone.View.extend({
        el: $("#ProyectosPanel"),
        tagName: "ul",
        initialize: function () {
            this.model.bind("reset", this.render, this);
            var self = this;
            this.model.bind("add", function (proyecto) {
                $(self.el).append(new vistas.ProyectoListItemView({ model: proyecto }).render().el);
            });
            this.model.bind("remove", this.render, this);
        },
        render: function (eventName) {
            $(this.el).empty();
            _.each(this.model.models, function (proyecto) {
                $(this.el).append(new vistas.ProyectoListItemView({model:proyecto}).render().el);
            }, this);
            this.afterRender();
        },
        afterRender: function () {
            $('.projectNode').draggable({
                revert: true,
                revertDuration: 0
            });
        }
    });
    vistas.ProyectoListItemView = Backbone.View.extend({
        tagName: "li",
        attributes: function () {
            return {
                'data-idproy': this.model.id
            };
        },
        events:{
            "click .edit":"editProyecto"
        },
        className:"projectNode fc-event fc-event-hori fc-event-draggable fc-event-start fc-event-end",
        template: Handlebars.compile($('#TemplateCajaProy').html()),
        initialize: function () {
            this.model.bind("change", this.render, this);
            this.model.bind("destroy", this.close, this);
        },
        render: function (eventName) {
            $(this.el).html(this.template(this.model.toJSON()));
            return this;
        },
        editProyecto: function (event) {
            App.navigate("proyectos/"+this.model.id, true);
            return false;
        },
        close: function () {
            $(this.el).unbind();
            $(this.el).remove();
        }
    });

    vistas.ProyectoView = Backbone.View.extend({
        template: Handlebars.compile($('#TemplateProyectoDetalle').html()),
        initialize: function () {
            this.model.bind("change", this.render, this);
        },

        render: function (eventName) {
            $(this.el).html(this.template(this.model.toJSON()));
            return this;
        },

        events: {
            "change input": "change",
            "click .save": "saveProyecto",
            "click .delete": "deleteProyecto",
            "click .cancel": "close"
        },

        change: function (event) {
            var target = event.target;
            console.log('changing ' + target.id + ' from: ' + target.defaultValue + ' to: ' + target.value);
            // You could change your model on the spot, like this:
            // var change = {};
            // change[target.name] = target.value;
            // this.model.set(change);
        },

        saveProyecto: function () {
            this.model.set({
                Nombre: $('#nombre_proyecto').val(),
                IdTipoProyecto: $('#tipoProyecto').val(),
                IdEstado: $('#estado').val(),
                IdResponsable: $('#responsable').val(),
            });
            if (this.model.isNew()) {
                var self = this;
                App.proyectos.create(this.model, {
                    success: function () {
                        self.close();
                    }
                });
            } else {
                var self = this;
                this.model.save(null,{
                    success: function (model, response) {
                        console.log("Intento cerrar");
                        self.close();
                        App.navigate("", true);
                    },
                    error: function (model, response) {
                        console.log(model);
                    },
                    wait: true
                });
                
            }
            return false;
        },

        deleteProyecto: function () {
            var self = this;
            var resQuitar = confirm("¿Desea eliminar este proyecto?");
            if (resQuitar) {
                var tmp1 = App.proyectos.get(this.model.id);
                App.proyectos.remove(tmp1);
                this.model.destroy({
                    success: function () {
                        self.close();
                    }
                ,wait:true});
            }
            return false;
        },
        close: function () {
            $(this.el).unbind();
            $(this.el).empty();
            App.navigate("", true);
        }
    });

    vistas.HeaderView = Backbone.View.extend({
        el: $("#HeaderPanel"),
        template: Handlebars.compile($('#TemplateHeader').html()),
        initialize: function () {
            this.render();
        },
        render: function (eventName) {
            $(this.el).html(this.template());
            return this;
        },
        events: {
            "click .nuevo": "newProyecto"
        },

        newProyecto: function (event) {
            App.navigate("proyectos/new", true);
            return false;
        }
    });

    //-------------------Terminan vistas backbone------------------------------------------
    var router = Backbone.Router.extend({
        routes: {
            "": "index",
            "proyectos/new": "newProyecto",
            "proyectos/:id": "proyectoDetails",
            '*path': 'index'
            
        },
        initialize:function(){
            this.Header = new vistas.HeaderView();
            this.Header.render();
        },
        index: function () {
            if (App.proyectoView) {
                App.proyectoView.close();
            }
            this.proyectos = new modelos.ProyectosCollection();
            var self = this;
            this.proyectos.fetch().then(function () {
                self.proyectosListView = new vistas.ProyectosListView({ model: self.proyectos });
                self.proyectosListView.render();
                if (self.requestedId) self.proyectoDetails(self.requestedId);
            });
        },
        proyectoDetails: function (id) {
            console.log("mostrar detalles");
            if (this.proyectos) {
                this.proyecto = App.proyectos.get(id);
                if (this.proyectoView) {
                    this.proyectoView.close();
                }
                this.proyectoView = new vistas.ProyectoView({ model: this.proyecto });
                $("#ProyContentPanel").html(this.proyectoView.render().el);
            }
            else {
                this.requestedId = id;
                this.defaultRoute();
            }
        },
        newProyecto: function () {
            if (App.proyectoView) {
                App.proyectoView.close();
            }
            App.proyectoView = new vistas.ProyectoView({ model: new modelos.Proyecto() });
            $("#ProyContentPanel").html(App.proyectoView.render().el);
        }

    });

    App = new router();
    Backbone.history.start({ pushState: true, root: rootUrl });
}


function removeEvent(eventId) {
    var resQuitar = confirm("¿Desea quitar esta actividad?");
    if (resQuitar)
    {
        var actividad = new modelos.Actividad({ id: eventId });
        actividad.destroy();
        $("#calendar").fullCalendar( 'removeEvents',eventId )
        //actividad.fetch();
        //model.destroy
    }
}