var dataTable;

$(document).ready(function () {
    loadList();
});

function loadList() {
    dataTable = $('#DT_load').DataTable({
        "ajax": {
            "url": "/api/menuitem",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "name", width: "25%" },
            { data: "price", render: $.fn.dataTable.render.number(',','.',2,"$"), width: "30%" },
            { data: "category.name", width: "25%" },
            { data: "foodType.name", width: "20%" },
            {
                data: "id", width: "30%",
                "render": function (data) {
                    return `<div class="text-center">
                            <a href="/Admin/MenuItems/Upsert?id=${data}"
                            class ="btn btn-success text-white style="cursor:pointer; width=100px;"> <i class="far fa-edit"></i>Edit</a>
                            <a onClick=Delete('/api/menuitem/'+${data})
                            class ="btn btn-danger text-white style="cursor:pointer; width=100px;"> <i class="far fa-trash-alt"></i>Delete</a>
                    </div>`;
                }
            }
        ],
        "language": {
            "emptyTable": "no data found."
        },
        "width": "100%"
    });
};

function Delete(url) {
    swal({
        title: "Are you sure you want to delete?",
        text: "You will not be able to restore this data!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}