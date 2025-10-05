var dataTable;

$(document).ready(function () {
    dataTable = $('#DT_load').DataTable({
        "ajax": {
            "url": "/api/rewardpoints",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "thresholdPoints", "width": "18%" },
            { "data": "description", "width": "34%" },
            { "data": "promoCode", "width": "18%" }, // string, not ID
            {
                "data": "isActive",
                "render": function (d) { return d ? "Yes" : "No"; },
                "width": "10%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center d-flex gap-2 justify-content-center">
                            <a href="/Admin/RewardPoints/Upsert?id=${data}" 
                               class="btn btn-success text-white" style="width:100px;">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>
                            <a onClick=Delete('/api/rewardpoints/${data}') 
                               class="btn btn-danger text-white" style="width:100px;">
                                <i class="bi bi-trash"></i> Delete
                            </a>
                        </div>`;
                },
                "width": "20%"
            }
        ],
        "language": {
            "emptyTable": "No reward point rules found."
        },
        "width": "100%"
    });
});

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
