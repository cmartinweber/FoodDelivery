var dataTable;

$(document).ready(function () {
    loadList();
});

function loadList() {
    dataTable = $('#DT_load').DataTable({
        "ajax": {
            "url": "/api/promocode",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "code", "width": "10%" },
            { "data": "description", "width": "15%" },
            {
                "data": "discountType",
                "render": function (data) {
                    return data == 0 ? "Percentage" : "Fixed Amount";
                },
                "width": "10%"
            },
            { "data": "discountValue", "width": "10%" },
            {
                "data": "validFrom",
                "render": function (data) {
                    return data ? new Date(data).toLocaleDateString() : "";
                },
                "width": "10%"
            },
            {
                "data": "validTo",
                "render": function (data) {
                    return data ? new Date(data).toLocaleDateString() : "";
                },
                "width": "10%"
            },
            { "data": "minimumOrderValue", "width": "10%" },
            { "data": "usesRemaining", "width": "10%" },
            {
                "data": "isActive",
                "render": function (data) {
                    return data ? "Yes" : "No";
                },
                "width": "5%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center d-flex gap-2 justify-content-center">
                            <a href="/Admin/PromoCodes/Upsert?id=${data}" 
                               class="btn btn-success text-white" style="width:100px;">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>
                            <a onClick=Delete('/api/promocode/${data}') 
                               class="btn btn-danger text-white" style="width:100px;">
                                <i class="bi bi-trash"></i> Delete
                            </a>
                        </div>`;
                },
                "width": "20%"
            }
        ],
        "language": {
            "emptyTable": "No promo codes found."
        },
        "width": "100%"
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won’t be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}
