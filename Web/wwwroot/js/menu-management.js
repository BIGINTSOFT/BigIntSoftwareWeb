$(document).ready(function() {
    // DataTable initialization
    var table = $('#menusTable').DataTable({
        "processing": true,
        "serverSide": false,
        "ajax": {
            "url": "/Menu/GetMenus",
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
                "data": "icon",
                "render": function(data, type, row) {
                    return data ? `<i class="${data}"></i> ${data}` : '-';
                }
            },
            { 
                "data": null,
                "render": function(data, type, row) {
                    var controller = row.controller || '';
                    var action = row.action || '';
                    return controller && action ? `${controller}/${action}` : '-';
                }
            },
            { 
                "data": "parentId",
                "render": function(data, type, row) {
                    return data ? `Menü ID: ${data}` : 'Ana Menü';
                }
            },
            { "data": "sortOrder" },
            { 
                "data": "isActive",
                "render": function(data, type, row) {
                    return data ? 
                        '<span class="badge bg-success">Aktif</span>' : 
                        '<span class="badge bg-danger">Pasif</span>';
                }
            },
            {
                "data": null,
                "orderable": false,
                "render": function(data, type, row) {
                    return `
                        <button class="btn btn-sm btn-outline-info view-permissions" data-id="${row.id}" title="Yetkileri Görüntüle">
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
                            <button class="btn btn-sm btn-outline-primary edit-menu" data-id="${row.id}">
                                <i class="bi bi-pencil"></i>
                            </button>
                            <button class="btn btn-sm btn-outline-danger delete-menu" data-id="${row.id}">
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

    // Add Menu Button
    $('#addMenuBtn').click(function() {
        $('#menuModalLabel').text('Yeni Menü Ekle');
        $('#menuForm')[0].reset();
        $('#menuId').val('');
        $('#menuModal').modal('show');
    });

    // Edit Menu Button
    $(document).on('click', '.edit-menu', function() {
        var menuId = $(this).data('id');
        loadMenu(menuId);
    });

    // Delete Menu Button
    $(document).on('click', '.delete-menu', function() {
        var menuId = $(this).data('id');
        if (confirm('Bu menüyü silmek istediğinizden emin misiniz?')) {
            deleteMenu(menuId);
        }
    });

    // Save Menu Button
    $('#saveMenuBtn').click(function() {
        saveMenu();
    });

    // Cancel Menu Button
    $('#cancelMenuBtn').click(function() {
        $('#menuModal').modal('hide');
    });

    // Load Menu Data
    function loadMenu(id) {
        console.log('🔍 Loading menu for edit, menuId:', id);
        $.ajax({
            url: '/Menu/GetMenu/' + id,
            type: 'GET',
            success: function(response) {
                console.log('✅ Menu edit data response:', response);
                if (response.success) {
                    var data = response.data;
                    $('#menuModalLabel').text('Menü Düzenle');
                    $('#menuId').val(data.id);
                    $('#menuName').val(data.name);
                    $('#menuDescription').val(data.description);
                    $('#menuIcon').val(data.icon);
                    $('#menuController').val(data.controller);
                    $('#menuAction').val(data.action);
                    $('#menuSortOrder').val(data.sortOrder);
                    $('#menuIsActive').prop('checked', data.isActive);
                    $('#menuIsVisible').prop('checked', data.isVisible);
                    $('#menuModal').modal('show');
                    console.log('✅ Menu edit form populated');
                } else {
                    console.error('❌ Menu edit error:', response.error);
                    showAlert('Menü bilgileri yüklenirken hata oluştu!', 'error');
                }
            },
            error: function(xhr, status, error) {
                console.error('❌ Menu edit AJAX error:', error);
                showAlert('Menü bilgileri yüklenirken hata oluştu!', 'error');
            }
        });
    }

    // Save Menu
    function saveMenu() {
        var formData = {
            Id: $('#menuId').val(),
            Name: $('#menuName').val(),
            Description: $('#menuDescription').val(),
            Icon: $('#menuIcon').val(),
            Controller: $('#menuController').val(),
            Action: $('#menuAction').val(),
            SortOrder: parseInt($('#menuSortOrder').val()) || 0,
            IsActive: $('#menuIsActive').is(':checked'),
            IsVisible: $('#menuIsVisible').is(':checked')
        };

        var url = formData.Id ? '/Menu/Update' : '/Menu/Create';
        var method = 'POST';

        $.ajax({
            url: url,
            type: method,
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function(response) {
                if (response.success) {
                    showAlert(response.message, 'success');
                    $('#menuModal').modal('hide');
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
                    showAlert('Menü kaydedilirken hata oluştu!', 'error');
                }
            }
        });
    }

    // Delete Menu
    function deleteMenu(id) {
        $.ajax({
            url: '/Menu/Delete/' + id,
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
                showAlert('Menü silinirken hata oluştu!', 'error');
            }
        });
    }

    // Form Validation
    $('#menuForm').on('submit', function(e) {
        e.preventDefault();
        saveMenu();
    });

    // View Permissions Button Click
    $(document).on('click', '.view-permissions', function() {
        var menuId = $(this).data('id');
        openMenuPermissionsModal(menuId);
    });

    // Menu Permissions Modal Functions
    function openMenuPermissionsModal(menuId) {
        $('#menuPermissionsModal').modal('show');
        
        // Menü bilgilerini yükle
        loadMenuInfo(menuId);
        
        // Kullanıcı yetkilerini yükle
        loadMenuUserPermissions(menuId);
        loadAvailableUsersForMenuPermission(menuId);
        
        // Rol yetkilerini yükle
        loadMenuRolePermissions(menuId);
        loadAvailableRolesForMenuPermission(menuId);
        
        // Store current menu ID
        $('#menuPermissionsModal').data('menu-id', menuId);
    }

    function loadMenuInfo(menuId) {
        $.ajax({
            url: '/Menu/GetMenu',
            type: 'GET',
            data: { id: menuId },
            success: function(response) {
                if (response.success) {
                    displayMenuInfo(response.data);
                } else {
                    showMenuModalAlert(response.error, 'error');
                }
            },
            error: function() {
                showMenuModalAlert('Menü bilgileri yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function displayMenuInfo(menu) {
        $('#modalMenuName').text(menu.name || '-');
        var controllerAction = '';
        if (menu.controller && menu.action) {
            controllerAction = `${menu.controller}/${menu.action}`;
        } else if (menu.controller) {
            controllerAction = menu.controller;
        } else {
            controllerAction = '-';
        }
        $('#modalMenuControllerAction').text(controllerAction);
        
        var statusBadge = menu.isActive ? 
            '<span class="badge bg-success">Aktif</span>' : 
            '<span class="badge bg-danger">Pasif</span>';
        $('#modalMenuStatus').html(statusBadge);
    }

    function showMenuModalAlert(message, type) {
        var alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
        $('#menuModalAlert')
            .removeClass('alert-success alert-danger')
            .addClass(alertClass)
            .find('#menuModalAlertMessage')
            .text(message);
        $('#menuModalAlert').show();
        
        // 3 saniye sonra otomatik gizle
        setTimeout(function() {
            $('#menuModalAlert').fadeOut();
        }, 3000);
    }

    // User Permissions Tab
    function loadMenuUserPermissions(menuId) {
        $.ajax({
            url: '/Menu/GetMenuUserPermissions',
            type: 'GET',
            data: { menuId: menuId },
            success: function(response) {
                if (response.success) {
                    displayAssignedUserPermissions(response.data);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Kullanıcı yetkileri yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function loadAvailableUsersForMenuPermission(menuId, search = '') {
        $.ajax({
            url: '/Menu/GetAvailableUsersForMenuPermission',
            type: 'GET',
            data: { menuId: menuId, search: search },
            success: function(response) {
                if (response.success) {
                    displayAvailableUserPermissions(response.data);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Kullanıcılar yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function displayAssignedUserPermissions(users) {
        var html = '';
        if (users.length === 0) {
            html = `
                <div class="empty-state">
                    <i class="bi bi-people"></i>
                    <p class="mb-0">Bu menüye yetkili kullanıcı bulunmuyor</p>
                </div>
            `;
        } else {
            users.forEach(function(user) {
                var fullName = ((user.firstName || '') + ' ' + (user.lastName || '')).trim();
                var sourceBadge = user.source === 'Role' ? 
                    '<span class="badge bg-primary me-2">Rol</span>' : 
                    '<span class="badge bg-success me-2">Direkt</span>';
                
                var removeButton = user.source === 'Direct' ? 
                    `<button class="btn btn-sm btn-outline-danger remove-user-permission" data-user-id="${user.id}" title="Yetkiyi Kaldır">
                        <i class="bi bi-x"></i>
                    </button>` : 
                    `<span class="text-muted small">Rol bazlı yetki</span>`;
                
                html += `
                    <div class="role-item d-flex justify-content-between align-items-center p-3 bg-light border rounded mb-2">
                        <div class="role-info">
                            <div class="d-flex align-items-center mb-1">
                                <i class="bi bi-person-circle text-primary me-2"></i>
                                <span class="fw-bold">${fullName || '-'}</span>
                                ${sourceBadge}
                            </div>
                            <div class="role-description">@${user.username} • ${user.email || '-'}</div>
                        </div>
                        <div class="role-actions">
                            ${removeButton}
                        </div>
                    </div>
                `;
            });
        }
        $('#assignedUsersPermissionsList').html(html);
    }

    function displayAvailableUserPermissions(users) {
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
                var fullName = ((user.firstName || '') + ' ' + (user.lastName || '')).trim();
                var statusBadge = user.isActive ? 
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
                            <div class="role-description">@${user.username} • ${user.email || '-'}</div>
                        </div>
                        <div class="role-actions">
                            <button class="btn btn-sm btn-outline-success add-user-permission" data-user-id="${user.id}" title="Yetki Ekle">
                                <i class="bi bi-plus"></i>
                            </button>
                        </div>
                    </div>
                `;
            });
        }
        $('#availableUsersPermissionsList').html(html);
    }

    // Role Permissions Tab
    function loadMenuRolePermissions(menuId) {
        $.ajax({
            url: '/Menu/GetMenuRolePermissions',
            type: 'GET',
            data: { menuId: menuId },
            success: function(response) {
                if (response.success) {
                    displayAssignedRolePermissions(response.data);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Rol yetkileri yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function loadAvailableRolesForMenuPermission(menuId, search = '') {
        $.ajax({
            url: '/Menu/GetAvailableRolesForMenuPermission',
            type: 'GET',
            data: { menuId: menuId, search: search },
            success: function(response) {
                if (response.success) {
                    displayAvailableRolePermissions(response.data);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Roller yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function displayAssignedRolePermissions(roles) {
        var html = '';
        if (roles.length === 0) {
            html = `
                <div class="empty-state">
                    <i class="bi bi-shield"></i>
                    <p class="mb-0">Bu menüye yetkili rol bulunmuyor</p>
                </div>
            `;
        } else {
            roles.forEach(function(role) {
                var statusBadge = role.isActive ? 
                    '<span class="badge bg-success me-2">Aktif</span>' : 
                    '<span class="badge bg-danger me-2">Pasif</span>';
                
                html += `
                    <div class="role-item d-flex justify-content-between align-items-center p-3 bg-light border rounded mb-2">
                        <div class="role-info">
                            <div class="d-flex align-items-center mb-1">
                                <i class="bi bi-shield-fill text-warning me-2"></i>
                                <span class="fw-bold">${role.name}</span>
                                ${statusBadge}
                            </div>
                            <div class="role-description">${role.description || 'Açıklama bulunmuyor'}</div>
                        </div>
                        <div class="role-actions">
                            <button class="btn btn-sm btn-outline-danger remove-role-permission" data-role-id="${role.id}" title="Yetkiyi Kaldır">
                                <i class="bi bi-x"></i>
                            </button>
                        </div>
                    </div>
                `;
            });
        }
        $('#assignedRolesPermissionsList').html(html);
    }

    function displayAvailableRolePermissions(roles) {
        var html = '';
        if (roles.length === 0) {
            html = `
                <div class="empty-state">
                    <i class="bi bi-shield-plus"></i>
                    <p class="mb-0">Eklenecek rol bulunamadı</p>
                </div>
            `;
        } else {
            roles.forEach(function(role) {
                var statusBadge = role.isActive ? 
                    '<span class="badge bg-success me-2">Aktif</span>' : 
                    '<span class="badge bg-danger me-2">Pasif</span>';
                
                html += `
                    <div class="role-item d-flex justify-content-between align-items-center p-3 bg-white border rounded mb-2">
                        <div class="role-info">
                            <div class="d-flex align-items-center mb-1">
                                <i class="bi bi-shield-fill text-info me-2"></i>
                                <span class="fw-bold">${role.name}</span>
                                ${statusBadge}
                            </div>
                            <div class="role-description">${role.description || 'Açıklama bulunmuyor'}</div>
                        </div>
                        <div class="role-actions">
                            <button class="btn btn-sm btn-outline-success add-role-permission" data-role-id="${role.id}" title="Yetki Ekle">
                                <i class="bi bi-plus"></i>
                            </button>
                        </div>
                    </div>
                `;
            });
        }
        $('#availableRolesPermissionsList').html(html);
    }

    // Tab Change Events
    $('#roles-tab').on('click', function() {
        var menuId = $('#menuPermissionsModal').data('menu-id');
        loadMenuRolePermissions(menuId);
        loadAvailableRolesForMenuPermission(menuId);
    });

    // User Permission Actions
    $(document).on('click', '.add-user-permission', function() {
        var userId = $(this).data('user-id');
        var menuId = $('#menuPermissionsModal').data('menu-id');
        
        $.ajax({
            url: '/Menu/AssignUserToMenu',
            type: 'POST',
            data: { menuId: menuId, userId: userId },
            success: function(response) {
                if (response.success) {
                    showAlert(response.message, 'success');
                    loadMenuUserPermissions(menuId);
                    loadAvailableUsersForMenuPermission(menuId);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Kullanıcı yetkisi verilirken hata oluştu!', 'error');
            }
        });
    });

    $(document).on('click', '.remove-user-permission', function() {
        var userId = $(this).data('user-id');
        var menuId = $('#menuPermissionsModal').data('menu-id');
        
        $.ajax({
            url: '/Menu/RemoveUserFromMenu',
            type: 'DELETE',
            data: { menuId: menuId, userId: userId },
            success: function(response) {
                if (response.success) {
                    showAlert(response.message, 'success');
                    loadMenuUserPermissions(menuId);
                    loadAvailableUsersForMenuPermission(menuId);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Kullanıcı yetkisi kaldırılırken hata oluştu!', 'error');
            }
        });
    });

    // Role Permission Actions
    $(document).on('click', '.add-role-permission', function() {
        var roleId = $(this).data('role-id');
        var menuId = $('#menuPermissionsModal').data('menu-id');
        
        $.ajax({
            url: '/Menu/AssignRoleToMenu',
            type: 'POST',
            data: { menuId: menuId, roleId: roleId },
            success: function(response) {
                if (response.success) {
                    showAlert(response.message, 'success');
                    loadMenuRolePermissions(menuId);
                    loadAvailableRolesForMenuPermission(menuId);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Rol yetkisi verilirken hata oluştu!', 'error');
            }
        });
    });

    $(document).on('click', '.remove-role-permission', function() {
        var roleId = $(this).data('role-id');
        var menuId = $('#menuPermissionsModal').data('menu-id');
        
        $.ajax({
            url: '/Menu/RemoveRoleFromMenu',
            type: 'DELETE',
            data: { menuId: menuId, roleId: roleId },
            success: function(response) {
                if (response.success) {
                    showAlert(response.message, 'success');
                    loadMenuRolePermissions(menuId);
                    loadAvailableRolesForMenuPermission(menuId);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Rol yetkisi kaldırılırken hata oluştu!', 'error');
            }
        });
    });

    // Search Functions
    $('#userPermissionSearchInput').on('input', function() {
        var search = $(this).val();
        var menuId = $('#menuPermissionsModal').data('menu-id');
        loadAvailableUsersForMenuPermission(menuId, search);
    });

    $('#rolePermissionSearchInput').on('input', function() {
        var search = $(this).val();
        var menuId = $('#menuPermissionsModal').data('menu-id');
        loadAvailableRolesForMenuPermission(menuId, search);
    });
});
