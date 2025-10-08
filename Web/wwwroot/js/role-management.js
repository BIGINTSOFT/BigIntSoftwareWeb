$(document).ready(function() {
    // DataTable initialization
    initRoleDataTable();
});

function initRoleDataTable() {
    // Destroy existing DataTable if it exists
    if ($.fn.DataTable.isDataTable('#rolesTable')) {
        $('#rolesTable').DataTable().destroy();
    }
    
    var table = $('#rolesTable').DataTable({
        "processing": true,
        "serverSide": false,
        "ajax": {
            "url": "/Role/GetRoles",
            "type": "GET",
            "dataSrc": function(json) {
                return json.data;
            },
            "error": function(xhr, error, thrown) {
                console.error('DataTables AJAX error:', error, thrown);
            }
        },
        "columns": [
            { "data": "Id" },
            { "data": "Name" },
            { "data": "Description" },
            { 
                "data": "IsActive",
                "render": function(data, type, row) {
                    return data ? 
                        '<span class="badge bg-success">Aktif</span>' : 
                        '<span class="badge bg-danger">Pasif</span>';
                }
            },
            { 
                "data": "CreatedDate",
                "render": function(data, type, row) {
                    return new Date(data).toLocaleDateString('tr-TR');
                }
            },
            {
                "data": null,
                "orderable": false,
                "render": function(data, type, row) {
                    return `
                        <button class="btn btn-sm btn-outline-primary role-permissions" data-role-id="${row.Id}" title="Yetkiler">
                            <i class="bi bi-shield-check"></i>
                        </button>
                    `;
                }
            },
            {
                "data": null,
                "orderable": false,
                "render": function(data, type, row) {
                    return `
                        <div class="btn-group" role="group">
                            <button class="btn btn-sm btn-outline-primary edit-role" data-id="${row.Id}">
                                <i class="bi bi-pencil"></i>
                            </button>
                            <button class="btn btn-sm btn-outline-danger delete-role" data-id="${row.Id}">
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
}

$(document).ready(function() {
    // Add Role Button
    $('#addRoleBtn').click(function() {
        $('#roleModalLabel').text('Yeni Rol Ekle');
        $('#roleForm')[0].reset();
        $('#roleId').val('');
        $('#roleModal').modal('show');
    });

    // Edit Role Button
    $(document).on('click', '.edit-role', function() {
        var roleId = $(this).data('id');
        loadRole(roleId);
    });

    // Delete Role Button
    $(document).on('click', '.delete-role', function() {
        var roleId = $(this).data('id');
        if (confirm('Bu rolü silmek istediğinizden emin misiniz?')) {
            deleteRole(roleId);
        }
    });

    // Save Role Button
    $('#saveRoleBtn').click(function() {
        saveRole();
    });

    // Cancel Role Button
    $('#cancelRoleBtn').click(function() {
        $('#roleModal').modal('hide');
    });

    // Load Role Data
    function loadRole(id) {
        console.log('🔍 Loading role for edit, roleId:', id);
        $.ajax({
            url: '/Role/GetRole/' + id,
            type: 'GET',
            success: function(response) {
                console.log('✅ Role edit data response:', response);
                if (response.success) {
                    var data = response.data;
                    $('#roleModalLabel').text('Rol Düzenle');
                    $('#roleId').val(data.Id);
                    $('#roleName').val(data.Name);
                    $('#roleDescription').val(data.Description);
                    $('#roleIsActive').prop('checked', data.IsActive);
                    $('#roleModal').modal('show');
                    console.log('✅ Role edit form populated');
                } else {
                    console.error('❌ Role edit error:', response.error);
                    showAlert('Rol bilgileri yüklenirken hata oluştu!', 'error');
                }
            },
            error: function(xhr, status, error) {
                console.error('❌ Role edit AJAX error:', error);
                showAlert('Rol bilgileri yüklenirken hata oluştu!', 'error');
            }
        });
    }

    // Save Role
    function saveRole() {
        var formData = {
            Id: $('#roleId').val(),
            Name: $('#roleName').val(),
            Description: $('#roleDescription').val(),
            IsActive: $('#roleIsActive').is(':checked')
        };

        var url = formData.Id ? '/Role/Update' : '/Role/Create';
        var method = 'POST';

        $.ajax({
            url: url,
            type: method,
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function(response) {
                if (response.success) {
                    showAlert(response.message, 'success');
                    $('#roleModal').modal('hide');
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
                    showAlert('Rol kaydedilirken hata oluştu!', 'error');
                }
            }
        });
    }

    // Delete Role
    function deleteRole(id) {
        $.ajax({
            url: '/Role/Delete/' + id,
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
                showAlert('Rol silinirken hata oluştu!', 'error');
            }
        });
    }

    // Form Validation
    $('#roleForm').on('submit', function(e) {
        e.preventDefault();
        saveRole();
    });

    // Role Permissions Button Click
    $(document).on('click', '.role-permissions', function() {
        var roleId = $(this).data('role-id');
        openRolePermissionsModal(roleId);
    });

    // Role Permissions Modal Functions
    function openRolePermissionsModal(roleId) {
        $('#rolePermissionsModal').modal('show');
        
        // Rol bilgilerini yükle
        loadRolePermissionsInfo(roleId);
        
        // Kullanıcıları yükle
        loadRoleUsers(roleId);
        loadAvailableUsers(roleId);
        
        // Menü yetkilerini yükle (Yeni ERP Yapısı)
        loadRoleMenuPermissions(roleId);
        loadAvailableMenusForRolePermission(roleId);
        
        // Store current role ID
        $('#rolePermissionsModal').data('role-id', roleId);
    }

    function loadRolePermissionsInfo(roleId) {
        console.log('🔍 Loading role permissions info for roleId:', roleId);
        $.ajax({
            url: '/Role/GetRole',
            type: 'GET',
            data: { id: roleId },
            success: function(response) {
                console.log('✅ Role permissions info response:', response);
                if (response.success) {
                    displayRolePermissionsInfo(response.data);
                } else {
                    console.error('❌ Role permissions info error:', response.error);
                    showRolePermissionsModalAlert(response.error, 'error');
                }
            },
            error: function(xhr, status, error) {
                console.error('❌ Role permissions info AJAX error:', error);
                showRolePermissionsModalAlert('Rol bilgileri yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function displayRolePermissionsInfo(role) {
        console.log('🔍 Displaying role permissions info:', role);
        $('#modalRolePermissionsName').text(role.Name || '-');
        $('#modalRolePermissionsDescription').text(role.Description || '-');
        console.log('✅ Role permissions info displayed');
    }

    function showRolePermissionsModalAlert(message, type) {
        var alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
        $('#rolePermissionsModalAlert')
            .removeClass('alert-success alert-danger')
            .addClass(alertClass)
            .find('#rolePermissionsModalAlertMessage')
            .text(message);
        $('#rolePermissionsModalAlert').show();
        
        // 3 saniye sonra otomatik gizle
        setTimeout(function() {
            $('#rolePermissionsModalAlert').fadeOut();
        }, 3000);
    }

    function loadRoleUsers(roleId) {
        $.ajax({
            url: '/Role/GetRoleUsers',
            type: 'GET',
            data: { roleId: roleId },
            success: function(response) {
                if (response.success) {
                    displayAssignedUsers(response.data);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Kullanıcılar yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function loadAvailableUsers(roleId, search = '') {
        $.ajax({
            url: '/Role/GetAvailableUsers',
            type: 'GET',
            data: { roleId: roleId, search: search },
            success: function(response) {
                if (response.success) {
                    displayAvailableUsers(response.data);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Kullanıcılar yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function displayAssignedUsers(users) {
        var html = '';
        if (users.length === 0) {
            html = `
                <div class="empty-state">
                    <i class="bi bi-people"></i>
                    <p class="mb-0">Bu role atanmış kullanıcı bulunmuyor</p>
                </div>
            `;
        } else {
            users.forEach(function(user) {
                var fullName = ((user.FirstName || '') + ' ' + (user.LastName || '')).trim();
                var statusBadge = user.IsActive ? 
                    '<span class="badge bg-success me-2">Aktif</span>' : 
                    '<span class="badge bg-danger me-2">Pasif</span>';
                
                html += `
                    <div class="role-item d-flex justify-content-between align-items-center p-3 bg-white border rounded mb-2">
                        <div class="role-info">
                            <div class="d-flex align-items-center mb-1">
                                <i class="bi bi-person-circle text-primary me-2"></i>
                                <span class="fw-bold">${fullName || '-'}</span>
                                ${statusBadge}
                            </div>
                            <div class="role-description">@${user.Username} • ${user.Email || '-'}</div>
                        </div>
                        <div class="role-actions">
                            <button class="btn btn-sm btn-outline-danger remove-user" data-user-id="${user.Id}" title="Kullanıcıyı Kaldır">
                                <i class="bi bi-x"></i>
                            </button>
                        </div>
                    </div>
                `;
            });
        }
        $('#assignedUsersList').html(html);
    }

    function displayAvailableUsers(users) {
        var html = '';
        if (users.length === 0) {
            html = `
                <div class="empty-state">
                    <i class="bi bi-person-plus"></i>
                    <p class="mb-0">Eklenecek kullanıcı bulunamadı</p>
                </div>
            `;
        } else {
            users.forEach(function(user) {
                var fullName = ((user.FirstName || '') + ' ' + (user.LastName || '')).trim();
                var statusBadge = user.IsActive ? 
                    '<span class="badge bg-success me-2">Aktif</span>' : 
                    '<span class="badge bg-danger me-2">Pasif</span>';
                
                html += `
                    <div class="role-item d-flex justify-content-between align-items-center p-3 bg-white border rounded mb-2">
                        <div class="role-info">
                            <div class="d-flex align-items-center mb-1">
                                <i class="bi bi-person-circle text-success me-2"></i>
                                <span class="fw-bold">${fullName || '-'}</span>
                                ${statusBadge}
                            </div>
                            <div class="role-description">@${user.Username} • ${user.Email || '-'}</div>
                        </div>
                        <div class="role-actions">
                            <button class="btn btn-sm btn-outline-success add-user" data-user-id="${user.Id}" title="Kullanıcıyı Ekle">
                                <i class="bi bi-plus"></i>
                            </button>
                        </div>
                    </div>
                `;
            });
        }
        $('#availableUsersList').html(html);
    }

    // Add User to Role
    $(document).on('click', '.add-user', function() {
        var userId = $(this).data('user-id');
        var roleId = $('#roleUsersModal').data('role-id');
        
        $.ajax({
            url: '/Role/AssignUserToRole',
            type: 'POST',
            data: { roleId: roleId, userId: userId },
            success: function(response) {
                if (response.success) {
                    showAlert(response.message, 'success');
                    loadRoleUsers(roleId);
                    loadAvailableUsers(roleId);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Kullanıcı atanırken hata oluştu!', 'error');
            }
        });
    });

    // Remove User from Role
    $(document).on('click', '.remove-user', function() {
        var userId = $(this).data('user-id');
        var roleId = $('#roleUsersModal').data('role-id');
        
        $.ajax({
            url: '/Role/RemoveUserFromRole',
            type: 'DELETE',
            data: { roleId: roleId, userId: userId },
            success: function(response) {
                if (response.success) {
                    showAlert(response.message, 'success');
                    loadRoleUsers(roleId);
                    loadAvailableUsers(roleId);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Kullanıcı çıkarılırken hata oluştu!', 'error');
            }
        });
    });

    // User Search
    $('#userSearchInput').on('input', function() {
        var search = $(this).val();
        var roleId = $('#roleUsersModal').data('role-id');
        loadAvailableUsers(roleId, search);
    });

    // Role Menu Permission Management (Yeni ERP Yapısı)
    $(document).on('click', '.add-role-menu-permission', function() {
        var menuId = $(this).data('menu-id');
        var permissionLevel = $(this).data('permission-level');
        var roleId = $('#rolePermissionsModal').data('role-id');
        var $button = $(this);
        
        // Show loading state
        $button.html('<span class="loading-spinner"></span>').prop('disabled', true);
        
        $.ajax({
            url: '/Role/AssignMenuPermissionToRole',
            type: 'POST',
            data: { roleId: roleId, menuId: menuId, permissionLevel: permissionLevel },
            success: function(response) {
                if (response.success) {
                    showRolePermissionsModalAlert(response.message, 'success');
                    loadRoleMenuPermissions(roleId);
                    loadAvailableMenusForRolePermission(roleId);
                } else {
                    showRolePermissionsModalAlert(response.error, 'error');
                }
            },
            error: function() {
                showRolePermissionsModalAlert('Menü yetkisi atanırken hata oluştu!', 'error');
            },
            complete: function() {
                // Reset button state
                $button.html('<i class="bi bi-plus"></i>').prop('disabled', false);
            }
        });
    });

    // Remove Menu Permission from Role
    $(document).on('click', '.remove-role-menu-permission', function() {
        var permissionId = $(this).data('permission-id');
        var roleId = $('#rolePermissionsModal').data('role-id');
        var $button = $(this);
        
        // Show loading state
        $button.html('<span class="loading-spinner"></span>').prop('disabled', true);
        
        $.ajax({
            url: '/Role/RemoveMenuPermissionFromRole',
            type: 'DELETE',
            data: { roleId: roleId, permissionId: permissionId },
            success: function(response) {
                if (response.success) {
                    showRolePermissionsModalAlert(response.message, 'success');
                    loadRoleMenuPermissions(roleId);
                    loadAvailableMenusForRolePermission(roleId);
                } else {
                    showRolePermissionsModalAlert(response.error, 'error');
                }
            },
            error: function() {
                showRolePermissionsModalAlert('Menü yetkisi kaldırılırken hata oluştu!', 'error');
            },
            complete: function() {
                // Reset button state
                $button.html('<i class="bi bi-x"></i>').prop('disabled', false);
            }
        });
    });

    // Role Menu Search
    $('#roleMenuSearch').on('input', function() {
        var search = $(this).val();
        var roleId = $('#rolePermissionsModal').data('role-id');
        loadAvailableMenusForRolePermission(roleId, search);
    });

    // Role Menu Permission Search
    $('#roleMenuSearch').on('input', function() {
        var search = $(this).val();
        var roleId = $('#rolePermissionsModal').data('role-id');
        loadAvailableMenusForRolePermission(roleId, search);
    });

    // Add System Permission to Role
    $(document).on('click', '.add-system-permission', function() {
        var permissionId = $(this).data('permission-id');
        var roleId = $('#rolePermissionsModal').data('role-id');
        
        $.ajax({
            url: '/Role/AssignSystemPermissionToRole',
            type: 'POST',
            data: { roleId: roleId, permissionId: permissionId },
            success: function(response) {
                if (response.success) {
                    showRolePermissionsModalAlert(response.message, 'success');
                    loadRoleSystemPermissions(roleId);
                    loadAvailableSystemPermissionsForRole(roleId);
                } else {
                    showRolePermissionsModalAlert(response.error, 'error');
                }
            },
            error: function() {
                showRolePermissionsModalAlert('Sistem yetkisi atanırken hata oluştu!', 'error');
            }
        });
    });

    // Remove System Permission from Role
    $(document).on('click', '.remove-system-permission', function() {
        var permissionId = $(this).data('permission-id');
        var roleId = $('#rolePermissionsModal').data('role-id');
        
        $.ajax({
            url: '/Role/RemoveSystemPermissionFromRole',
            type: 'DELETE',
            data: { roleId: roleId, permissionId: permissionId },
            success: function(response) {
                if (response.success) {
                    showRolePermissionsModalAlert(response.message, 'success');
                    loadRoleSystemPermissions(roleId);
                    loadAvailableSystemPermissionsForRole(roleId);
                } else {
                    showRolePermissionsModalAlert(response.error, 'error');
                }
            },
            error: function() {
                showRolePermissionsModalAlert('Sistem yetkisi kaldırılırken hata oluştu!', 'error');
            }
        });
    });

});

// Role Menu Permission Management Functions (Yeni ERP Yapısı)

function loadRoleMenuPermissions(roleId) {
    $.ajax({
        url: '/Role/GetRoleMenuPermissions',
        type: 'GET',
        data: { roleId: roleId },
        success: function(response) {
            if (response.success) {
                displayAssignedRoleMenuPermissions(response.data);
            } else {
                showRolePermissionsModalAlert(response.error, 'error');
            }
        },
        error: function() {
            showRolePermissionsModalAlert('Menü yetkileri yüklenirken hata oluştu!', 'error');
        }
    });
}

function displayAssignedRoleMenuPermissions(permissions) {
    var html = '';
    if (permissions.length === 0) {
        html = `
            <div class="empty-state">
                <i class="bi bi-list-ul"></i>
                <p class="mb-0">Bu role atanmış menü yetkisi bulunmuyor</p>
            </div>
        `;
    } else {
        permissions.forEach(function(permission) {
            var permissionBadge = getPermissionBadgeClass(permission.PermissionLevel);
            
            html += `
                <div class="d-flex justify-content-between align-items-center p-3 bg-light border rounded mb-2">
                    <div class="menu-info">
                        <div class="d-flex align-items-center mb-1">
                            <i class="${permission.MenuIcon || 'bi bi-circle'} text-primary me-2"></i>
                            <span class="fw-bold">${permission.MenuName}</span>
                            <span class="badge ${permissionBadge} ms-2">${permission.PermissionLevel}</span>
                        </div>
                        <div class="menu-description">${permission.MenuController}/${permission.MenuAction || ''}</div>
                    </div>
                    <button class="btn btn-sm btn-outline-danger remove-role-menu-permission" data-permission-id="${permission.Id}" title="Yetkiyi Kaldır">
                        <i class="bi bi-x"></i>
                    </button>
                </div>
            `;
        });
    }
    $('#assignedRoleMenuPermissionsList').html(html);
}

function loadAvailableMenusForRolePermission(roleId, search = '') {
    $.ajax({
        url: '/Role/GetAvailableMenusForRolePermission',
        type: 'GET',
        data: { roleId: roleId, search: search },
        success: function(response) {
            if (response.success) {
                displayAvailableMenusForRolePermission(response.data);
            } else {
                showRolePermissionsModalAlert(response.error, 'error');
            }
        },
        error: function() {
            showRolePermissionsModalAlert('Menüler yüklenirken hata oluştu!', 'error');
        }
    });
}

function displayAvailableMenusForRolePermission(menus) {
    var html = '';
    if (menus.length === 0) {
        html = '<div class="text-muted text-center py-3">Eklenecek menü bulunamadı</div>';
    } else {
        menus.forEach(function(menu) {
            html += `
                <div class="d-flex justify-content-between align-items-center p-2 border-bottom">
                    <div>
                        <div class="fw-bold">
                            <i class="${menu.Icon || 'bi bi-circle'} me-2"></i>
                            ${menu.Name}
                        </div>
                        <small class="text-muted">${menu.Controller}/${menu.Action || ''}</small>
                    </div>
                    <div class="btn-group" role="group">
                        <button class="btn btn-sm btn-outline-success add-role-menu-permission" data-menu-id="${menu.Id}" data-permission-level="VIEW">
                            <i class="bi bi-eye"></i> VIEW
                        </button>
                        <button class="btn btn-sm btn-outline-primary add-role-menu-permission" data-menu-id="${menu.Id}" data-permission-level="CREATE">
                            <i class="bi bi-plus"></i> CREATE
                        </button>
                        <button class="btn btn-sm btn-outline-warning add-role-menu-permission" data-menu-id="${menu.Id}" data-permission-level="EDIT">
                            <i class="bi bi-pencil"></i> EDIT
                        </button>
                        <button class="btn btn-sm btn-outline-danger add-role-menu-permission" data-menu-id="${menu.Id}" data-permission-level="DELETE">
                            <i class="bi bi-trash"></i> DELETE
                        </button>
                    </div>
                </div>
            `;
        });
    }
    $('#availableMenusForRolePermissionList').html(html);
}

function showRoleMenuModalAlert(message, type) {
    var alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
    $('#roleMenuModalAlert')
        .removeClass('alert-success alert-danger')
        .addClass(alertClass)
        .find('#roleMenuModalAlertMessage')
        .text(message);
    $('#roleMenuModalAlert').show();
    
    // 3 saniye sonra otomatik gizle
    setTimeout(function() {
        $('#roleMenuModalAlert').fadeOut();
    }, 3000);
}

// System Permissions Management Functions

function loadRoleSystemPermissions(roleId) {
    $.ajax({
        url: '/Role/GetRoleSystemPermissions',
        type: 'GET',
        data: { roleId: roleId },
        success: function(response) {
            if (response.success) {
                displayAssignedRoleSystemPermissions(response.data);
            } else {
                showRolePermissionsModalAlert(response.error, 'error');
            }
        },
        error: function() {
            showRolePermissionsModalAlert('Sistem yetkileri yüklenirken hata oluştu!', 'error');
        }
    });
}

function displayAssignedRoleSystemPermissions(permissions) {
    var html = '';
    if (!permissions || permissions.length === 0) {
        html = `
            <div class="empty-state">
                <i class="bi bi-shield"></i>
                <p class="mb-0">Bu role atanmış sistem yetkisi bulunmuyor</p>
            </div>
        `;
    } else {
        permissions.forEach(function(permission) {
            var code = permission.code || permission.Code || 'UNKNOWN';
            var name = permission.name || permission.Name || code;
            var description = permission.description || permission.Description || '';
            var id = permission.id || permission.Id;
            var badgeClass = getPermissionBadgeClass(code);
            html += `
                <div class="d-flex justify-content-between align-items-center p-3 bg-light border rounded mb-2">
                    <div class="permission-info">
                        <div class="d-flex align-items-center mb-1">
                            <span class="badge ${badgeClass} me-2">${code}</span>
                            <span class="fw-bold">${name}</span>
                        </div>
                        <div class="permission-description">${description}</div>
                    </div>
                    <button class="btn btn-sm btn-outline-danger remove-system-permission" data-permission-id="${id}" title="Yetkiyi Kaldır">
                        <i class="bi bi-x"></i>
                    </button>
                </div>
            `;
        });
    }
    $('#assignedRolePermissionsList').html(html);
}

function loadAvailableSystemPermissionsForRole(roleId, search = '') {
    $.ajax({
        url: '/Role/GetAvailableSystemPermissionsForRole',
        type: 'GET',
        data: { roleId: roleId, search: search },
        success: function(response) {
            if (response.success) {
                displayAvailableRoleSystemPermissions(response.data);
            } else {
                showRolePermissionsModalAlert(response.error, 'error');
            }
        },
        error: function() {
            showRolePermissionsModalAlert('Mevcut sistem yetkileri yüklenirken hata oluştu!', 'error');
        }
    });
}

function displayAvailableRoleSystemPermissions(permissions) {
    var html = '';
    if (!permissions || permissions.length === 0) {
        html = '<div class="text-muted text-center py-3">Eklenecek sistem yetkisi bulunamadı</div>';
    } else {
        permissions.forEach(function(permission) {
            var code = permission.code || permission.Code || 'UNKNOWN';
            var name = permission.name || permission.Name || code;
            var description = permission.description || permission.Description || '';
            var id = permission.id || permission.Id;
            var badgeClass = getPermissionBadgeClass(code);
            html += `
                <div class="d-flex justify-content-between align-items-center p-2 border-bottom">
                    <div>
                        <div class="fw-bold">
                            <span class="badge ${badgeClass} me-2">${code}</span>
                            ${name}
                        </div>
                        <small class="text-muted">${description}</small>
                    </div>
                    <button class="btn btn-sm btn-outline-success add-system-permission" data-permission-id="${id}">
                        <i class="bi bi-plus"></i>
                    </button>
                </div>
            `;
        });
    }
    $('#availableRolePermissionsList').html(html);
}

function getPermissionBadgeClass(permissionCode) {
    switch(permissionCode) {
        case 'VIEW': return 'bg-primary';
        case 'CREATE': return 'bg-success';
        case 'EDIT': return 'bg-warning text-dark';
        case 'UPDATE': return 'bg-warning text-dark';
        case 'DELETE': return 'bg-danger';
        default: return 'bg-secondary';
    }
}

// Reload correct lists on tab switch
$(document).on('shown.bs.tab', 'button[data-bs-toggle="tab"]', function (e) {
    var target = $(e.target).attr('data-bs-target');
    var roleId = $('#rolePermissionsModal').data('role-id');
    if (!roleId) return;
    if (target === '#role-users') {
        loadRoleUsers(roleId);
        loadAvailableUsers(roleId);
    } else if (target === '#role-menus') {
        loadRoleMenuPermissions(roleId);
        loadAvailableMenusForRolePermission(roleId);
    }
});

// DataTable'ı yeniden başlat
function refreshRoleDataTable() {
    initRoleDataTable();
}
