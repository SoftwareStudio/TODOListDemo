/*global document: false, confirm: false, $, jQuery */
var refreshList, refreshEvents;
refreshList = function (data) {
    "use strict";
    if (!data.Success) {
        $("#msgError p").html(data.Msg);
        $("#msgError").show().delay(4000).fadeOut();
    } else {
        $("#msgSuccess p").html(data.Msg);
        $("#msgSuccess").show().delay(4000).fadeOut();
        var section = $("#section_info").attr("section");
        $("#todo_list").load("/TodoItem/GetTODOs/", { section: section }, function () {
            $("#sortable").sortable({ axis: "y" });
        });
    }
};

refreshEvents = function (data) {
    "use strict";
    $('#modalTodo').modal('hide');
    $('#calendar').fullCalendar('refetchEvents');
    refreshList(data);
};


$(document).ready(function () {
    "use strict";
    $.ajaxSetup({
        cache: false
    });
    $("#msgSuccess").hide();
    $("#msgError").hide();
    //C-Create
    $(document).on("click", '#new_todo', function () {
        var date = new Date();
        $('#modalTodo').modal('show').load("/TodoItem/Create/", { date: date.toJSON() });
    });
    //R-Read
    $(document).on("click", '.todo_details', function () {
        var id = $(this).closest("li").attr("id");
        $('#modalTodo').modal('show').load("/TodoItem/Details/" + id);
    });
    //U-Update
    $(document).on("click", '.edit_todo', function () {
        var id = $(this).closest("li").attr("id");
        $('#modalTodo').modal('show').load("/TodoItem/Edit/" + id);
    });

    $(document).on("click", '.finish_todo', function () {
        var r, id;
        r = confirm('Are you sure you want check this task as finished? Checking will remove it from active list and put it in archive.', 'Check finish?');
        if (r === true) {
            id = $(this).closest("li").attr("id");
            $.post("/TodoItem/Finish/" + id,
                        function (data) {
                    refreshList(data);
                });
        }
    });
    //D-Delete
    $(document).on("click", '#delete_todo', function () {
        var r, id;
        r = confirm('Are you sure you want to delete this task?', 'Delete task?');
        if (r === true) {
            id = $(this).attr("todoid");
            $.post("/TodoItem/Delete/" + id,
                        function (data) {
                    refreshEvents(data);
                });
        }
    });

    //Submit changes
    $(document).on("click", '#submit_todo', function () {
        jQuery.ajax({
            type: "POST",
            url: "/TodoItem/Save/",
            data: $("#form_todo").serialize(),
            traditional: true,
            success: function (data) {
                refreshEvents(data);
            }
        });
    });

    //Save new order of task list
    $(document).on("click", '#submit-list', function () {
        $.ajax({
            url: '/TodoItem/SortTODOs/',
            data: { items: $("#sortable").sortable('toArray') },
            type: 'post',
            traditional: true,
            success: function (data) {
                refreshList(data);
            }
        });
    });

    //Lazy loading more tasks in list
    $(document).on("click", '#todo_more', function () {
        var section = parseInt($("#section_info").attr("section"), 10) + 1;
        if ($("#view_archive").attr("id") === "view_archive") {
            $("#todo_list").load("/TodoItem/GetTODOs/", { section: section }, function () {
                $("#sortable").sortable({ axis: "y" });
            });
        } else {
            $("#todo_list").load("/TodoItem/GetArchive/", { section: section });
        }
    });

    //Show archived or ongoing list
    $(document).on("click", '#view_archive', function () {
		var section = 1;
        $(this).attr("id", "view_active");
        $(this).text("(View active list)");
        $("#todo_list").load("/TodoItem/GetArchive/", { section: section });
    });
    $(document).on("click", '#view_active', function () {
		var section = 1;
        $(this).attr("id", "view_archive");
        $(this).text("(View archived list)");
        $("#todo_list").load("/TodoItem/GetTODOs/", { section: section }, function () {
            $("#sortable").sortable({ axis: "y" });
        });
    });

    //Show and configure fullcalendar.js
    $('#calendar').fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,agendaDay'
        },
        allDayDefault: false,
        selectable: true,
        selectHelper: true,
        events: "/TodoItem/GetCalendarEvents/",
        eventClick: function (calEvent) {
            $('#modalTodo').modal('show').load("/TodoItem/Edit/" + calEvent.id);
        },
        select: function (start) {
            $('#modalTodo').modal('show').load("/TodoItem/Create/", { date: start.toJSON() });
        }
    });

    //Load todo list
    $("#todo_list").load("/TodoItem/GetTODOs/", function () {
        $("#sortable").sortable({ axis: "y" });
    });

});