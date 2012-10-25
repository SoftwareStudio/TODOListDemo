$(document).ready(function () {

    $("#msgSuccess").hide();
    $("#msgError").hide();
    //C-Create
    $(document).on("click", '#new_todo', function (e) {
        var date = new Date();
        $('#modalTodo').modal('show').load("/TodoItem/Create/", { date: date.toJSON() });
    });
    //R-Read
    $(document).on("click", '.todo_details', function (e) {
        var id = $(this).closest("li").attr("id");
        $('#modalTodo').modal('show').load("/TodoItem/Details/" + id);
    });
    //U-Update
    $(document).on("click", '.edit_todo', function (e) {
        var id = $(this).closest("li").attr("id");
        $('#modalTodo').modal('show').load("/TodoItem/Edit/" + id);
    });

    $(document).on("click", '.finish_todo', function (e) {
        var r = confirm('Are you sure you want check this task as finished? Checking will remove it from the list', 'Check finish?');
        if (r == true) {
            var id = $(this).closest("li").attr("id");
            $.post("/TodoItem/Finish/" + id,
                        function (data) {
                            RefreshList(data);
                        });
        }
    });
    //D-Delete
    $(document).on("click", '#delete_todo', function (e) {
        var r = confirm('Are you sure you want to delete this task?', 'Delete task?');
        if (r == true) {
            var id = $(this).attr("todoid");
            $.post("/TodoItem/Delete/" + id,
                        function (data) {
                            RefreshEvents(data);
                        });
        }
    });

    //Submit changes
    $(document).on("click", '#submit_todo', function (e) {
        jQuery.ajax({
            type: "POST",
            url: "/TodoItem/Save/",
            data: $("#form_todo").serialize(),
            traditional: true,
            success: function (data) {
                RefreshEvents(data);
            }
        });
    });

    //Save new order of task list
    $(document).on("click", '#submit-list', function (e) {
        $.ajax({
            url: '/TodoItem/SortTODOs/',
            data: { items: $("#sortable").sortable('toArray') },
            type: 'post',
            traditional: true,
            success: function (data) {
                RefreshList(data);
            }
        });
    });

    //Lazy loading more tasks in list
    $(document).on("click", '#todo_more', function (e) {
        var section = parseInt($("#section_info").attr("section")) + 1;
        if ($("#view_archive").attr("id") == "view_archive") {
            $("#todo_list").load("/TodoItem/GetTODOs/", { section: section }, function () {
                $("#sortable").sortable({ axis: "y" });
            });
        } else {
            $("#todo_list").load("/TodoItem/GetArchive/", { section: section });
        }
    });

    //Show archived or ongoing list
    $(document).on("click", '#view_archive', function (e) {
        $(this).attr("id", "view_active");
        $(this).text("(View active list)");
        $("#todo_list").load("/TodoItem/GetArchive/");
    });
    $(document).on("click", '#view_active', function (e) {
        $(this).attr("id", "view_archive");
        $(this).text("(View archived list)");
        $("#todo_list").load("/TodoItem/GetTODOs/", function () {
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
        eventClick: function (calEvent, jsEvent, view) {
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

function RefreshEvents(data) {
    $('#modalTodo').modal('hide');
    $('#calendar').fullCalendar('refetchEvents');
    RefreshList(data);    
}

function RefreshList(data) {
    if (!data.Success) {
        $("#msgError p").html(data.Msg);
        $("#msgError").show().delay(3000).fadeOut();
    } else {
        $("#msgSuccess p").html(data.Msg);
        $("#msgSuccess").show().delay(3000).fadeOut();
        var section = $("#section_info").attr("section");
        $("#todo_list").load("/TodoItem/GetTODOs/", { section: section }, function () {
            $("#sortable").sortable({ axis: "y" });
        });
    }
}