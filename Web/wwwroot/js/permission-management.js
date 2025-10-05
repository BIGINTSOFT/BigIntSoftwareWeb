$(document).ready(function() {
    // DataTable initialization
    var table = $('#permissionsTable').DataTable({
        "processing": true,
        "serverSide": false,
        "ajax": {
            "url": "/Permission/GetPermissions",
            "type": "GET",
            "dataSrc": function(json) {
                return json.data;
            },
            "error": function(xhr, error, thrown) {
                console.error('DataTables AJAX error:', error, thrown);
            }
        },
        "columns": [
            { "data": "id" },
            { "data": "name" },
            { 
                "data": "code",
                "render": function(data, type, row) {
                    return `<code>${data}</code>`;
                }
            },
            { "data": "description" },
            { 
                "data": "isActive",
                "render": function(data, type, row) {
                    return data ? 
                        '<span class="badge bg-success">Aktif</span>' : 
                        '<span class="badge bg-danger">Pasif</span>';
                }
            },
            { 
                "data": "createdDate",
                "render": function(data, type, row) {
                    return new Date(data).toLocaleDateString('tr-TR');
                }
            },
            {
                "data": null,
                "orderable": false,
                "render": function(data, type, row) {
                    return `
                        <div class="btn-group" role="group">
                            <button class="btn btn-sm btn-outline-primary edit-permission" data-id="${row.id}">
                                <i class="bi bi-pencil"></i>
                            </button>
                            <button class="btn btn-sm btn-outline-danger delete-permission" data-id="${row.id}">
                                <i class="bi bi-trash"></i>
                            </button>
                        </div>
                    `;
                }
            }
        ],
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.13.7/i18n/tr.json"
        }
    });

    // Add Permission Button
    $('#addPermissionBtn').click(function() {
        $('#permissionModalLabel').text('Yeni Yetki Ekle');
        $('#permissionForm')[0].reset();
        $('#permissionId').val('');
        $('#permissionModal').modal('show');
    });

    // Edit Permission Button
    $(document).on('click', '.edit-permission', function() {
        var permissionId = $(this).data('id');
        loadPermission(permissionId);
    });

    // Delete Permission Button
    $(document).on('click', '.delete-permission', function() {
        var permissionId = $(this).data('id');
        if (confirm('Bu yetkiyi silmek istediğinizden emin misiniz?')) {
            deletePermission(permissionId);
        }
    });

    // Save Permission Button
    $('#savePermissionBtn').click(function() {
        savePermission();
    });

    // Cancel Permission Button
    $('#cancelPermissionBtn').click(function() {
        $('#permissionModal').modal('hide');
    });

    // Load Permission Data
    function loadPermission(id) {
        console.log('🔍 Loading permission for edit, permissionId:', id);
        $.ajax({
            url: '/Permission/GetPermission/' + id,
            type: 'GET',
            success: function(response) {
                console.log('✅ Permission edit data response:', response);
                if (response.success) {
                    var data = response.data;
                    console.log('🔍 Permission data object:', data);
                    console.log('🔍 Data properties:', {
                        id: data.id,
                        name: data.name,
                        code: data.code,
                        description: data.description,
                        isActive: data.isActive
                    });
                    
                    $('#permissionModalLabel').text('Yetki Düzenle');
                    $('#permissionId').val(data.id);
                    $('#permissionName').val(data.name);
                    $('#permissionCode').val(data.code);
                    $('#permissionDescription').val(data.description);
                    $('#permissionIsActive').prop('checked', data.isActive);
                    
                    console.log('🔍 Form field values after setting:', {
                        permissionId: $('#permissionId').val(),
                        permissionName: $('#permissionName').val(),
                        permissionCode: $('#permissionCode').val(),
                        permissionDescription: $('#permissionDescription').val(),
                        permissionIsActive: $('#permissionIsActive').prop('checked')
                    });
                    
                    $('#permissionModal').modal('show');
                    console.log('✅ Permission edit form populated');
                } else {
                    console.error('❌ Permission edit error:', response.error);
                    showAlert('Yetki bilgileri yüklenirken hata oluştu!', 'error');
                }
            },
            error: function(xhr, status, error) {
                console.error('❌ Permission edit AJAX error:', error);
                showAlert('Yetki bilgileri yüklenirken hata oluştu!', 'error');
            }
        });
    }

    // Save Permission
    function savePermission() {
        var formData = {
            Id: $('#permissionId').val(),
            Name: $('#permissionName').val(),
            Code: $('#permissionCode').val(),
            Description: $('#permissionDescription').val(),
            IsActive: $('#permissionIsActive').is(':checked')
        };

        var url = formData.Id ? '/Permission/Update' : '/Permission/Create';
        var method = 'POST';

        $.ajax({
            url: url,
            type: method,
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function(response) {
                if (response.success) {
                    showAlert(response.message, 'success');
                    $('#permissionModal').modal('hide');
                    table.ajax.reload();
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function(xhr) {
                var response = xhr.responseJSON;
                if (response && response.errors) {
                    showAlert(response.errors.join('<br>'), 'error');
                } else {
                    showAlert('Yetki kaydedilirken hata oluştu!', 'error');
                }
            }
        });
    }

    // Delete Permission
    function deletePermission(id) {
        $.ajax({
            url: '/Permission/Delete/' + id,
            type: 'DELETE',
            success: function(response) {
                if (response.success) {
                    showAlert(response.message, 'success');
                    table.ajax.reload();
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Yetki silinirken hata oluştu!', 'error');
            }
        });
    }

    // Form Validation
    $('#permissionForm').on('submit', function(e) {
        e.preventDefault();
        savePermission();
    });
});
