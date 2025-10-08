// Role Management JavaScript with DevExtreme - Complete Implementation
$(document).ready(function() {
    initializeRoleManagement();
});

function initializeRoleManagement() {
    let rolesDataGrid;
    let currentRoleId = null;
    let isEditMode = false;

    // Initialize DevExtreme DataGrid
    function initDataGrid() {
        rolesDataGrid = $("#rolesDataGrid").dxDataGrid({
            dataSource: {
                load: function(loadOptions) {
                    return $.ajax({
                        url: '/Role/GetRoles',
                        type: 'GET',
                        dataType: 'json'
                    }).then(function(response) {
                        return {
                            data: response.data || [],
                            totalCount: response.data ? response.data.length : 0
                        };
                    });
                }
            },
            showBorders: true,
            showRowLines: true,
            showColumnLines: false,
            rowAlternationEnabled: true,
            hoverStateEnabled: true,
            allowColumnReordering: true,
            allowColumnResizing: true,
            columnAutoWidth: true,
            wordWrapEnabled: true,
            columnHidingEnabled: false,
            scrolling: { 
                columnRenderingMode: 'virtual', 
                showScrollbar: 'onHover',
                mode: 'standard'
            },
            columnChooser: {
                enabled: true,
                mode: 'select'
            },
            searchPanel: {
                visible: true,
                width: 240,
                placeholder: 'Ara...'
            },
            filterRow: {
                visible: true
            },
            headerFilter: {
                visible: true
            },
            paging: {
                pageSize: 20,
                pageSizes: [10, 20, 50, 100]
            },
            pager: {
                showPageSizeSelector: true,
                allowedPageSizes: [10, 20, 50, 100],
                showInfo: true,
                showNavigationButtons: true
            },
            export: {
                enabled: true,
                fileName: 'RolListesi',
                allowExportSelectedData: true
            },
            selection: {
                mode: 'multiple'
            },
            columns: [
                {
                    dataField: 'Id',
                    caption: 'ID',
                    width: 60,
                    alignment: 'center'
                },
                {
                    dataField: 'Name',
                    caption: 'Rol Adı',
                    width: 150
                },
                {
                    dataField: 'Description',
                    caption: 'Açıklama',
                    width: 200
                },
                {
                    dataField: 'IsActive',
                    caption: 'Durum',
                    width: 80,
                    alignment: 'center',
                    cellTemplate: function(container, options) {
                        const isActive = options.value;
                        const badgeClass = isActive ? 'dx-badge-success' : 'dx-badge-danger';
                        const text = isActive ? 'Aktif' : 'Pasif';
                        container.append(`<span class="dx-badge ${badgeClass}">${text}</span>`);
                    }
                },
                {
                    caption: 'Menü Yetkileri',
                    width: 80,
                    alignment: 'center',
                    allowSorting: false,
                    allowFiltering: false,
                    allowHeaderFiltering: false,
                    cellTemplate: function(container, options) {
                        const role = options.data;
                        const button = $('<button>')
                            .addClass('dx-action-btn dx-action-btn-permissions')
                            .attr('title', 'Menü Yetkileri')
                            .html('<i class="bi bi-gear"></i>')
                            .on('click', function() {
                                // Ensure roleId is a clean integer
                                const cleanRoleId = parseInt(role.Id);
                                console.log('Opening role permissions modal for roleId:', cleanRoleId);
                                openRolePermissionsModal(cleanRoleId, role.Name, role.Description, role.IsActive);
                            });
                        container.append(button);
                    }
                },
                {
                    dataField: 'CreatedDate',
                    caption: 'Oluşturulma',
                    width: 120,
                    dataType: 'datetime',
                    format: 'dd/MM/yyyy HH:mm'
                },
                {
                    caption: 'İşlemler',
                    width: 100,
                    alignment: 'center',
                    allowSorting: false,
                    allowFiltering: false,
                    allowHeaderFiltering: false,
                    cellTemplate: function(container, options) {
                        const role = options.data;
                        const actionsContainer = $('<div>').addClass('dx-action-buttons');



                        if (window.rolePermissions && window.rolePermissions.canView) {
                            const viewBtn = $('<button>')
                                .addClass('dx-action-btn dx-action-btn-view')
                                .attr('title', 'Görüntüle')
                                .html('<i class="bi bi-eye"></i>')
                                .on('click', function() {
                                    viewRole(parseInt(role.Id));
                                });
                            actionsContainer.append(viewBtn);
                        }
                        
                        if (window.rolePermissions && window.rolePermissions.canEdit) {
                            const editBtn = $('<button>')
                                .addClass('dx-action-btn dx-action-btn-edit')
                                .attr('title', 'Düzenle')
                                .html('<i class="bi bi-pencil"></i>')
                                .on('click', function() {
                                    editRole(parseInt(role.Id));
                                });
                            actionsContainer.append(editBtn);
                        }
                        
                        if (window.rolePermissions && window.rolePermissions.canDelete) {
                            const deleteBtn = $('<button>')
                                .addClass('dx-action-btn dx-action-btn-delete')
                                .attr('title', 'Sil')
                                .html('<i class="bi bi-trash"></i>')
                                .on('click', function() {
                                    deleteRole(parseInt(role.Id));
                                });
                            actionsContainer.append(deleteBtn);
                        }
                        
                        container.append(actionsContainer);
                    }
                }
            ],
            onRowClick: function(e) {
                // Row click event if needed
            },
            onSelectionChanged: function(e) {
                // Selection changed event if needed
            },
            onExporting: function(e) {
                // Custom export logic if needed
            }
        }).dxDataGrid('instance');
    }

    // Add Role Button
    $('#addRoleBtn').click(function() {
        if (!window.rolePermissions || !window.rolePermissions.canCreate) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        isEditMode = false;
        currentRoleId = null;
        $('#roleModalLabel').text('Yeni Rol');
        $('#roleForm')[0].reset();
        $('#roleId').val('');
        showSlideModal();
    });

    // Refresh Button
    $('#refreshBtn').click(function() {
        rolesDataGrid.refresh();
        showAlert('Tablo yenilendi', 'success');
    });

    // Export Button
    $('#exportBtn').click(function() {
        try {
            if (rolesDataGrid && typeof rolesDataGrid.exportToExcel === 'function') {
                rolesDataGrid.exportToExcel({
                    fileName: 'RolListesi_' + new Date().toISOString().split('T')[0],
                    autoFilterEnabled: true
                });
            } else {
                showAlert('Excel export özelliği şu anda kullanılamıyor.', 'warning');
            }
        } catch (error) {
            console.error('Export error:', error);
            showAlert('Excel export sırasında hata oluştu.', 'error');
        }
    });

    // Role Action Functions
    function viewRole(roleId) {
        if (!window.rolePermissions || !window.rolePermissions.canView) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        $.get('/Role/GetRole/' + roleId)
            .done(function(response) {
                if (response.data) {
                    const role = response.data;
                    showRoleDetails(role);
                } else {
                    showAlert('Rol bilgileri alınamadı', 'error');
                }
            })
            .fail(function() {
                showAlert('Rol bilgileri alınamadı', 'error');
            });
    }

    function editRole(roleId) {
        if (!window.rolePermissions || !window.rolePermissions.canEdit) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        isEditMode = true;
        currentRoleId = roleId;
        
        $.get('/Role/GetRole/' + roleId)
            .done(function(response) {
                if (response.data) {
                    $('#roleModalLabel').text('Rol Düzenle');
                    $('#roleId').val(response.data.Id);
                    $('#roleName').val(response.data.Name);
                    $('#roleDescription').val(response.data.Description);
                    $('#roleIsActive').prop('checked', response.data.IsActive);
                    showSlideModal();
                } else {
                    showAlert('Rol bilgileri alınamadı', 'error');
                }
            })
            .fail(function() {
                showAlert('Rol bilgileri alınamadı', 'error');
            });
    }

    function deleteRole(roleId) {
        if (!window.rolePermissions || !window.rolePermissions.canDelete) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        currentRoleId = roleId;
        $('#deleteModal').modal('show');
    }

    // Save Role
    $('#saveRoleBtn').click(function() {
        if (validateForm()) {
            const formData = {
                Id: $('#roleId').val() || 0,
            Name: $('#roleName').val(),
            Description: $('#roleDescription').val(),
            IsActive: $('#roleIsActive').is(':checked')
        };

            const url = isEditMode ? '/Role/Update' : '/Role/Create';
            const method = 'POST';

        $.ajax({
            url: url,
            type: method,
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function(response) {
                if (response.success) {
                        hideSlideModal();
                        rolesDataGrid.refresh();
                    showAlert(response.message, 'success');
                } else {
                        showAlert(response.message || response.error, 'error');
                }
            },
            error: function(xhr) {
                    const response = xhr.responseJSON;
                    showAlert(response ? (response.message || response.error) : 'Bir hata oluştu', 'error');
            }
        });
    }
    });

    // Confirm Delete
    $('#confirmDeleteBtn').click(function() {
        $.ajax({
            url: '/Role/Delete/' + currentRoleId,
            type: 'DELETE',
            success: function(response) {
                if (response.success) {
                    $('#deleteModal').modal('hide');
                    rolesDataGrid.refresh();
                    showAlert(response.message, 'success');
                } else {
                    showAlert(response.message || response.error, 'error');
                }
            },
            error: function(xhr) {
                const response = xhr.responseJSON;
                showAlert(response ? (response.message || response.error) : 'Bir hata oluştu', 'error');
            }
        });
    });

    // Form Validation
    function validateForm() {
        let isValid = true;
        
        // Clear previous validation
        $('.form-control').removeClass('is-invalid');
        $('.invalid-feedback').text('');

        // Required fields
        const requiredFields = ['roleName'];

        requiredFields.forEach(function(field) {
            const value = $('#' + field).val().trim();
            if (!value) {
                $('#' + field).addClass('is-invalid');
                $('#' + field).siblings('.invalid-feedback').text('Bu alan gereklidir');
                isValid = false;
            }
        });

        return isValid;
    }

    // Slide Modal Functions
    function showSlideModal() {
        const modal = $('#roleModal');
        const modalDialog = modal.find('.modal-dialog-slide');
        
        // Add backdrop
        $('body').append('<div class="modal-backdrop fade show" id="modalBackdrop"></div>');
        
        // Show modal
        modal.addClass('show');
        modal.attr('aria-hidden', 'false');
        modal.css('display', 'block');
        
        // Trigger slide animation
        setTimeout(function() {
            modalDialog.addClass('show');
        }, 10);
        
        // Prevent body scroll
        $('body').addClass('modal-open');
    }
    
    function hideSlideModal() {
        const modal = $('#roleModal');
        const modalDialog = modal.find('.modal-dialog-slide');
        
        // Hide slide animation
        modalDialog.removeClass('show');
        
        // Wait for animation to complete
        setTimeout(function() {
            modal.removeClass('show');
            modal.attr('aria-hidden', 'true');
            modal.css('display', 'none');
            
            // Remove backdrop
            $('#modalBackdrop').remove();
            
            // Restore body scroll
            $('body').removeClass('modal-open');
        }, 300);
    }
    
    // Close modal on backdrop click
    $(document).on('click', '#modalBackdrop', function() {
        hideSlideModal();
    });
    
    // Close modal on escape key
    $(document).on('keydown', function(e) {
        if (e.key === 'Escape' && $('#roleModal').hasClass('show')) {
            hideSlideModal();
        }
    });
    
    // Close modal on close button
    $(document).on('click', '.btn-close', function() {
        hideSlideModal();
    });
    
    // Close modal on cancel button
    $(document).on('click', '#cancelRoleBtn', function() {
        hideSlideModal();
    });

    function showAlert(message, type) {
        const alertClass = type === 'success' ? 'alert-success' : (type === 'warning' ? 'alert-warning' : 'alert-danger');
        const alertHtml = '<div class="alert ' + alertClass + ' alert-dismissible fade show" role="alert">' +
            message +
            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
            '</div>';
        
        // Remove existing alerts
        $('.alert').remove();
        
        // Add new alert
        $('.card-body').prepend(alertHtml);
        
        // Auto remove after 5 seconds
        setTimeout(function() {
            $('.alert').fadeOut();
        }, 5000);
    }

    // Role Permissions Management Functions
    function openRolePermissionsModal(roleId, roleName, roleDescription, isActive) {
        $('#rolePermissionsModal').data('role-id', roleId);
        $('#modalRolePermissionsName').text(roleName);
        $('#modalRolePermissionsDescription').text(roleDescription);
        
        loadRoleMenuPermissions(roleId);
        loadAvailableMenusForRolePermission(roleId);
        $('#rolePermissionsModal').modal('show');
    }

    function loadRoleMenuPermissions(roleId) {
        console.log('Loading role menu permissions for roleId:', roleId, 'Type:', typeof roleId);
        
        // Ensure roleId is a clean integer
        const cleanRoleId = parseInt(roleId);
        console.log('Clean roleId:', cleanRoleId);
        
        $.ajax({
            url: '/Role/GetRoleMenuPermissions',
            type: 'GET',
            data: { roleId: cleanRoleId },
            success: function(response) {
                console.log('Role menu permissions response:', response);
                if (response.success) {
                    // Remove duplicates based on Id
                    const uniquePermissions = response.data.filter((permission, index, self) => 
                        index === self.findIndex(p => p.Id === permission.Id)
                    );
                    console.log('Unique permissions after deduplication:', uniquePermissions);
                    displayAssignedRoleMenuPermissions(uniquePermissions);
                } else {
                    showRolePermissionsModalAlert(response.message || response.error, 'error');
                }
            },
            error: function(xhr) {
                console.error('Error loading role menu permissions:', xhr);
                showRolePermissionsModalAlert('Menü yetkileri yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function loadAvailableMenusForRolePermission(roleId, search = '') {
        console.log('Loading available menus for roleId:', roleId, 'search:', search);
        
        // Ensure roleId is a clean integer
        const cleanRoleId = parseInt(roleId);
        
        $.ajax({
            url: '/Role/GetAvailableMenusForRolePermission',
            type: 'GET',
            data: { roleId: cleanRoleId, search: search },
            success: function(response) {
                console.log('Available menus response:', response);
                if (response.success) {
                    // Remove duplicates based on Id
                    const uniqueMenus = response.data.filter((menu, index, self) => 
                        index === self.findIndex(m => m.Id === menu.Id)
                    );
                    console.log('Unique menus after deduplication:', uniqueMenus);
                    displayAvailableMenusForRolePermission(uniqueMenus);
                } else {
                    showRolePermissionsModalAlert(response.message || response.error, 'error');
                }
            },
            error: function(xhr) {
                console.error('Error loading available menus:', xhr);
                showRolePermissionsModalAlert('Menüler yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function displayAssignedRoleMenuPermissions(permissions) {
        var html = '';
        if (permissions.length === 0) {
            html = `
                <div class="empty-state text-center py-4">
                    <i class="bi bi-gear fs-1 text-muted"></i>
                    <p class="mb-0 text-muted small">Bu role atanmış menü yetkisi bulunmuyor</p>
                </div>
            `;
        } else {
            // Menülere göre grupla
            const groupedByMenu = {};
            permissions.forEach(function(permission) {
                const menuId = permission.MenuId;
                if (!groupedByMenu[menuId]) {
                    groupedByMenu[menuId] = {
                        menuName: permission.MenuName || 'Bilinmeyen Menü',
                        menuIcon: permission.MenuIcon || 'bi bi-circle',
                        menuController: permission.MenuController || '',
                        menuAction: permission.MenuAction || '',
                        permissions: []
                    };
                }
                groupedByMenu[menuId].permissions.push({
                    id: permission.Id,
                    permissionId: permission.PermissionId,
                    level: permission.PermissionLevel,
                    name: permission.PermissionName,
                    notes: permission.Notes
                });
            });
            
            // Her menü için kart oluştur
            for (const menuId in groupedByMenu) {
                const menuData = groupedByMenu[menuId];
                const permBadges = menuData.permissions.map(p => 
                    `<span class="badge ${getPermissionBadgeClass(p.level)} me-1 small">${p.level}</span>`
                ).join('');
                
                html += `
                    <div class="card mb-2 border-0 shadow-sm">
                        <div class="card-body p-2">
                            <div class="d-flex justify-content-between align-items-start">
                                <div class="flex-grow-1">
                            <div class="d-flex align-items-center mb-1">
                                        <i class="${menuData.menuIcon} text-primary me-2"></i>
                                        <span class="fw-bold small">${menuData.menuName}</span>
                            </div>
                                    <div class="text-muted" style="font-size: 10px;">${menuData.menuController}/${menuData.menuAction}</div>
                                    <div class="mt-1">${permBadges}</div>
                        </div>
                                <div class="ms-2">
                                    <button class="btn btn-sm btn-outline-warning edit-role-menu-permissions me-1" 
                                            data-menu-id="${menuId}"
                                            data-menu-name="${menuData.menuName}"
                                            title="Yetkileri Düzenle">
                                        <i class="bi bi-pencil"></i>
                                    </button>
                                    <button class="btn btn-sm btn-outline-danger remove-all-role-menu-permissions" 
                                            data-menu-id="${menuId}"
                                            title="Tüm Yetkileri Kaldır">
                                        <i class="bi bi-trash"></i>
                            </button>
                                </div>
                            </div>
                        </div>
                    </div>
                `;
        }
        }
        $('#assignedRoleMenuPermissionsList').html(html);
    }

    function displayAvailableMenusForRolePermission(menus) {
        var html = '';
        if (menus.length === 0) {
            html = '<div class="text-muted text-center py-3 small">Eklenecek menü bulunamadı</div>';
        } else {
            menus.forEach(function(menu) {
                html += `
                    <div class="d-flex justify-content-between align-items-center p-2 border rounded mb-1 bg-white">
                        <div class="flex-grow-1">
                            <div class="fw-bold d-flex align-items-center small">
                                <i class="${menu.Icon || 'bi bi-circle'} me-2 text-primary"></i>
                                ${menu.Name}
                            </div>
                            <small class="text-muted" style="font-size: 11px;">${menu.Controller}/${menu.Action || ''}</small>
                        </div>
                        <div class="ms-2">
                            <button class="btn btn-sm btn-success add-role-menu-permission" 
                                    data-menu-id="${menu.Id}" 
                                    data-menu-name="${menu.Name}"
                                    title="Menü ve Yetki Ekle">
                                <i class="bi bi-plus-circle me-1"></i>
                                Ekle
                            </button>
                        </div>
                    </div>
                `;
            });
        }
        $('#availableMenusForRolePermissionList').html(html);
    }

    // Role Menu Permission Actions
    $(document).on('click', '.add-role-menu-permission', function() {
        var menuId = $(this).data('menu-id');
        var menuName = $(this).data('menu-name');
        var roleId = $('#rolePermissionsModal').data('role-id');
        
        console.log('Add role menu permission clicked:', { menuId, menuName, roleId });
        
        if (!menuId || !roleId) {
            showRolePermissionsModalAlert('Menü veya rol bilgisi bulunamadı', 'error');
            return;
        }
        
        // Show menu and permission selection modal (sağdan sola açılır)
        showRoleMenuPermissionSelectionModal(menuId, menuName, roleId);
    });

    // Edit permissions for a specific menu
    $(document).on('click', '.edit-role-menu-permissions', function() {
        var menuId = $(this).data('menu-id');
        var menuName = $(this).data('menu-name');
        var roleId = $('#rolePermissionsModal').data('role-id');
        
        // Get current permissions for this menu
        $.ajax({
            url: '/Role/GetRoleMenuPermissions',
            type: 'GET',
            data: { roleId: roleId },
            success: function(response) {
                if (response.success) {
                    // Filter permissions for this specific menu
                    var menuPermissions = response.data.filter(p => p.MenuId === menuId);
                    
                    if (menuPermissions.length > 0) {
                        // Get all permissions (not just available ones) for editing
                        $.ajax({
                            url: '/User/GetPermissions',
                            type: 'GET',
                            success: function(permissionsResponse) {
                                if (permissionsResponse.success) {
                                    // Show edit modal with all permissions and current permissions
                                    showEditRoleMenuPermissionModal(menuId, menuName, roleId, menuPermissions, permissionsResponse.data);
                } else {
                                    showRolePermissionsModalAlert('Yetki bilgileri alınamadı', 'error');
                                }
                            },
                            error: function(xhr) {
                                showRolePermissionsModalAlert('Yetki bilgileri alınamadı', 'error');
                            }
                        });
                } else {
                        showRolePermissionsModalAlert('Bu menü için atanmış yetki bulunamadı', 'warning');
                    }
                } else {
                    showRolePermissionsModalAlert('Yetki bilgileri alınamadı', 'error');
                }
            },
            error: function() {
                showRolePermissionsModalAlert('Yetki bilgileri alınamadı', 'error');
            }
        });
    });

    // Remove all permissions for a specific menu
    $(document).on('click', '.remove-all-role-menu-permissions', function() {
        var menuId = $(this).data('menu-id');
        var roleId = $('#rolePermissionsModal').data('role-id');
        var $button = $(this);
        
        if (confirm('Bu menüye ait tüm yetkileri kaldırmak istediğinizden emin misiniz?')) {
        // Show loading state
            $button.html('<span class="spinner-border spinner-border-sm"></span>').prop('disabled', true);
        
        $.ajax({
                url: '/Role/RemoveAllMenuPermissionsFromRole',
            type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ 
                    roleId: roleId, 
                    menuId: menuId
                }),
            success: function(response) {
                if (response.success) {
                    showRolePermissionsModalAlert(response.message, 'success');
                    loadRoleMenuPermissions(roleId);
                    loadAvailableMenusForRolePermission(roleId);
                } else {
                        showRolePermissionsModalAlert(response.message || response.error, 'error');
                }
            },
            error: function() {
                    showRolePermissionsModalAlert('Menü yetkileri kaldırılırken hata oluştu!', 'error');
            },
            complete: function() {
                // Reset button state
                    $button.html('<i class="bi bi-trash"></i>').prop('disabled', false);
                }
            });
        }
    });

    // Role Menu Search
    $('#roleMenuSearch').on('input', function() {
        var search = $(this).val();
        var roleId = $('#rolePermissionsModal').data('role-id');
        loadAvailableMenusForRolePermission(roleId, search);
    });
        
    // Role Menu Permission Selection Modal
    function showRoleMenuPermissionSelectionModal(menuId, menuName, roleId) {
        console.log('showRoleMenuPermissionSelectionModal called with:', { menuId, menuName, roleId });
        
        // Get all permissions for this menu
        $.ajax({
            url: '/User/GetPermissions',
            type: 'GET',
            success: function(permissionsResponse) {
                console.log('Permissions response:', permissionsResponse);
                if (permissionsResponse.success) {
                    buildAndShowRolePermissionModal(menuId, menuName, roleId, permissionsResponse.data, 'add');
                } else {
                    showRolePermissionsModalAlert('Yetki bilgileri alınamadı', 'error');
                }
            },
            error: function(xhr) {
                console.error('Error loading permissions:', xhr);
                showRolePermissionsModalAlert('Yetki bilgileri alınamadı', 'error');
            }
        });
    }

    // Edit Role Menu Permission Modal
    function showEditRoleMenuPermissionModal(menuId, menuName, roleId, currentPermissions, allPermissions) {
        buildAndShowRolePermissionModal(menuId, menuName, roleId, allPermissions, 'edit', currentPermissions);
    }
    
    // Build and show modal with dynamic permissions - SLIDE FROM RIGHT
    function buildAndShowRolePermissionModal(menuId, menuName, roleId, permissions, mode = 'add', currentPermissions = []) {
        console.log('buildAndShowRolePermissionModal called with:', {
            menuId, menuName, roleId, 
            permissionsCount: permissions.length,
            mode, 
            currentPermissionsCount: currentPermissions.length
        });
        // Permission checkboxları dinamik olarak oluştur
        let permissionCheckboxes = '';
        const iconMap = {
            'VIEW': 'bi-eye text-primary',
            'CREATE': 'bi-plus text-success',
            'EDIT': 'bi-pencil text-warning',
            'UPDATE': 'bi-pencil text-warning',
            'DELETE': 'bi-trash text-danger',
            'EXPORT': 'bi-download text-info',
            'IMPORT': 'bi-upload text-info',
            'PRINT': 'bi-printer text-secondary',
            'MANAGE': 'bi-gear text-dark'
        };
        
        // Mevcut permission'ları kontrol etmek için set oluştur
        const currentPermissionIds = new Set(currentPermissions.map(p => p.PermissionId));
        
        permissions.forEach(function(permission, index) {
            const permName = permission.Name || permission.name || '';
            const permCode = permission.Code || permission.code || permName;
            const permDesc = permission.Description || permission.description || '';
            const permId = permission.Id || permission.id;
            const icon = iconMap[permCode.toUpperCase()] || 'bi-shield text-secondary';
            
            // Düzenleme modunda mevcut yetkileri kontrol et
            let isChecked = '';
            if (mode === 'edit') {
                // PermissionId ile kontrol et
                const hasPermission = currentPermissionIds.has(permId);
                // Eğer PermissionId bulunamazsa, PermissionLevel ile kontrol et
                const hasPermissionByLevel = currentPermissions.some(p => p.PermissionLevel === permCode);
                isChecked = (hasPermission || hasPermissionByLevel) ? 'checked' : '';
            }
            
            permissionCheckboxes += `
                <div class="form-check">
                    <input class="form-check-input permission-checkbox" type="checkbox" 
                           id="permission_${permId}" 
                           value="${permCode}" 
                           data-permission-id="${permId}"
                           ${isChecked}>
                    <label class="form-check-label" for="permission_${permId}">
                        <i class="${icon} me-1"></i>
                        <strong>${permName}</strong>${permDesc ? ' - ' + permDesc : ''}
                    </label>
                </div>
            `;
        });
        
        const modalHtml = `
            <div class="modal fade" id="roleMenuPermissionModal" tabindex="-1" data-bs-backdrop="static" data-bs-keyboard="false">
                <div class="modal-dialog modal-dialog-slide-right modal-xl">
                    <div class="modal-content">
                        <div class="modal-header bg-primary text-white">
                            <h5 class="modal-title">
                                <i class="bi bi-gear me-2"></i>
                                ${mode === 'edit' ? 'Rol Menü Yetkilerini Düzenle' : 'Rol Menü ve Yetki Seçimi'}
                            </h5>
                            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <div class="row">
                                <div class="col-md-6">
                                    <h6 class="text-primary">
                                        <i class="bi bi-list-ul me-2"></i>
                                        Seçilen Menü
                                    </h6>
                                    <div class="card border-primary">
                                        <div class="card-body">
                                            <div class="d-flex align-items-center">
                                                <i class="bi bi-folder me-2 text-primary"></i>
                                                <div>
                                                    <strong>${menuName}</strong>
                                                    <br>
                                                    <small class="text-muted">Menü ID: ${menuId}</small>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <h6 class="text-success">
                                        <i class="bi bi-shield-check me-2"></i>
                                        Yetki Seviyeleri
                                    </h6>
                                    <div class="card border-success">
                                        <div class="card-body" style="max-height: 300px; overflow-y: auto;">
                                            ${permissionCheckboxes || '<p class="text-muted small">Yetki bulunamadı</p>'}
                                        </div>
                                    </div>
                                </div>
                            </div>
                            
                            <div class="mt-3">
                                <label for="reasonInput" class="form-label">
                                    <i class="bi bi-chat-text me-1"></i>
                                    Sebep <small class="text-muted">(İsteğe bağlı)</small>
                                </label>
                                <textarea class="form-control" id="reasonInput" rows="3" 
                                          placeholder="Bu yetkileri verme sebebinizi yazın... (İsteğe bağlı)">${mode === 'edit' && currentPermissions.length > 0 ? currentPermissions[0].Notes || '' : ''}</textarea>
                            </div>
                            
                            <div class="mt-3">
                                <div class="alert alert-info">
                                    <i class="bi bi-info-circle me-2"></i>
                                    <strong>Bilgi:</strong> ${mode === 'edit' ? 'Seçtiğiniz yetkiler bu menü için rolün yetkilerini güncelleyecektir.' : 'Seçtiğiniz yetkiler bu menü için role atanacaktır.'}
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                                <i class="bi bi-x-lg me-1"></i>
                                İptal
                            </button>
                            <button type="button" class="btn btn-primary" onclick="${mode === 'edit' ? 'updateRoleMenuPermissions' : 'assignMultipleRoleMenuPermissions'}(${menuId}, ${roleId})">
                                <i class="bi bi-check-lg me-1"></i>
                                ${mode === 'edit' ? 'Yetkileri Güncelle' : 'Yetkileri Ata'}
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `;
        
        // Remove existing modal if any
        $('#roleMenuPermissionModal').remove();
        
        // Add new modal to body
        console.log('Adding modal to DOM...');
        $('body').append(modalHtml);
        console.log('Modal added to DOM, showing modal...');
        
        // Hide the parent modal first
        try {
            $('#rolePermissionsModal').modal('hide');
            console.log('Parent modal hidden');
        } catch (e) {
            console.warn('Error hiding parent modal:', e);
        }
        
        // Show modal with slide animation from right
        const modal = $('#roleMenuPermissionModal');
        const modalDialog = modal.find('.modal-dialog-slide-right');
        
        console.log('Modal elements found:', { modal: modal.length, modalDialog: modalDialog.length });
        
        // Remove any existing backdrop
        $('#rolePermissionModalBackdrop').remove();
        
        // Add backdrop
        $('body').append('<div class="modal-backdrop fade show" id="rolePermissionModalBackdrop"></div>');
        console.log('Backdrop added');
        
        // Show modal
        modal.addClass('show');
        modal.attr('aria-hidden', 'false');
        modal.css('display', 'block');
        console.log('Modal classes and styles applied');
        
        // Trigger slide animation
        setTimeout(function() {
            modalDialog.addClass('show');
            console.log('Slide animation triggered');
        }, 10);
        
        // Prevent body scroll
        $('body').addClass('modal-open');
        console.log('Modal should be visible now');
        
        // Remove modal from DOM when hidden
        modal.on('hidden.bs.modal', function() {
            $('#rolePermissionModalBackdrop').remove();
            $('body').removeClass('modal-open');
            // Show the parent modal again
            try {
                $('#rolePermissionsModal').modal('show');
            } catch (e) {
                console.warn('Error showing parent modal:', e);
            }
            $(this).remove();
        });
        
        // Close on backdrop click
        $(document).on('click', '#rolePermissionModalBackdrop', function() {
            hideRolePermissionModal();
        });
        
        // Close button handler
        modal.find('.btn-close, [data-bs-dismiss="modal"]').on('click', function() {
            hideRolePermissionModal();
        });
    }
    
    // Hide role permission modal with animation
    function hideRolePermissionModal() {
        const modal = $('#roleMenuPermissionModal');
        const modalDialog = modal.find('.modal-dialog-slide-right');
        
        // Hide slide animation
        modalDialog.removeClass('show');
        
        // Wait for animation to complete
        setTimeout(function() {
            modal.removeClass('show');
            modal.attr('aria-hidden', 'true');
            modal.css('display', 'none');
            
            // Remove backdrop
            $('#rolePermissionModalBackdrop').remove();
            
            // Restore body scroll
            $('body').removeClass('modal-open');
            
            // Show the parent modal again
            try {
                $('#rolePermissionsModal').modal('show');
            } catch (e) {
                console.warn('Error showing parent modal:', e);
            }
            
            // Trigger hidden event
            modal.trigger('hidden.bs.modal');
        }, 300);
    }

    // Global function for assigning multiple role menu permissions
    window.assignMultipleRoleMenuPermissions = function(menuId, roleId) {
        const reason = $('#reasonInput').val().trim();
        
        // Get selected permissions from dynamic checkboxes
        const selectedPermissions = [];
        $('.permission-checkbox:checked').each(function() {
            const permissionId = $(this).data('permission-id');
            const permissionCode = $(this).val();
            const permissionName = $(this).next('label').find('strong').text();
            
            selectedPermissions.push({
                id: permissionId,
                code: permissionCode,
                name: permissionName
        });
    });

        if (selectedPermissions.length === 0) {
            alert('En az bir yetki seçmelisiniz!');
            return;
        }
        
        // Close modal with animation
        hideRolePermissionModal();
        
        // Show loading
        showRolePermissionsModalAlert(`${selectedPermissions.length} yetki atanıyor...`, 'info');
        
        // Assign permissions one by one
        let completed = 0;
        let errors = [];
        
        selectedPermissions.forEach(function(permission, index) {
        $.ajax({
                url: '/Role/AssignMenuPermissionToRole',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    roleId: roleId,
                    menuId: menuId,
                    permissionId: permission.id,
                    permissionLevel: permission.code,
                    notes: reason
                }),
            success: function(response) {
                    completed++;
                    if (!response.success) {
                        errors.push(`${permission.name}: ${response.message || response.error}`);
                    }
                    
                    // Check if all permissions are processed
                    if (completed === selectedPermissions.length) {
                        if (errors.length === 0) {
                            showRolePermissionsModalAlert(`${selectedPermissions.length} yetki başarıyla atandı!`, 'success');
                } else {
                            showRolePermissionsModalAlert(`${completed - errors.length} yetki atandı, ${errors.length} hata oluştu`, 'warning');
                        }
                        loadRoleMenuPermissions(roleId);
                        loadAvailableMenusForRolePermission(roleId);
                    }
                },
                error: function(xhr) {
                    completed++;
                    errors.push(`${permission.name}: Sunucu hatası`);
                    
                    if (completed === selectedPermissions.length) {
                        if (errors.length === selectedPermissions.length) {
                            showRolePermissionsModalAlert('Tüm yetkiler atanırken hata oluştu!', 'error');
                        } else {
                            showRolePermissionsModalAlert(`${completed - errors.length} yetki atandı, ${errors.length} hata oluştu`, 'warning');
                        }
                        loadRoleMenuPermissions(roleId);
                        loadAvailableMenusForRolePermission(roleId);
                    }
            }
        });
    });
    };

    // Global function for updating role menu permissions
    window.updateRoleMenuPermissions = function(menuId, roleId) {
        const reason = $('#reasonInput').val().trim();
        
        // Get selected permissions from dynamic checkboxes
        const selectedPermissions = [];
        $('.permission-checkbox:checked').each(function() {
            const permissionId = $(this).data('permission-id');
            const permissionCode = $(this).val();
            const permissionName = $(this).next('label').find('strong').text();
            
            selectedPermissions.push({
                id: permissionId,
                code: permissionCode,
                name: permissionName
            });
        });
        
        // Close modal with animation
        hideRolePermissionModal();
        
        // Show loading
        showRolePermissionsModalAlert('Yetkiler güncelleniyor...', 'info');
        
        // First, remove all existing permissions for this menu
    $.ajax({
            url: '/Role/RemoveAllMenuPermissionsFromRole',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ 
                roleId: roleId, 
                menuId: menuId
            }),
            success: function(removeResponse) {
                if (removeResponse.success) {
                    // Then add selected permissions
                    if (selectedPermissions.length > 0) {
                        let completed = 0;
                        let errors = [];
                        
                        selectedPermissions.forEach(function(permission, index) {
                            $.ajax({
                                url: '/Role/AssignMenuPermissionToRole',
                                type: 'POST',
                                contentType: 'application/json',
                                data: JSON.stringify({
                                    roleId: roleId,
                                    menuId: menuId,
                                    permissionId: permission.id,
                                    permissionLevel: permission.code,
                                    notes: reason
                                }),
        success: function(response) {
                                    completed++;
                                    if (!response.success) {
                                        errors.push(`${permission.name}: ${response.message || response.error}`);
                                    }
                                    
                                    if (completed === selectedPermissions.length) {
                                        if (errors.length === 0) {
                                            showRolePermissionsModalAlert(`${selectedPermissions.length} yetki başarıyla güncellendi!`, 'success');
            } else {
                                            showRolePermissionsModalAlert(`${completed - errors.length} yetki güncellendi, ${errors.length} hata oluştu`, 'warning');
                                        }
                                        loadRoleMenuPermissions(roleId);
                                        loadAvailableMenusForRolePermission(roleId);
                                    }
                                },
                                error: function(xhr) {
                                    completed++;
                                    errors.push(`${permission.name}: Sunucu hatası`);
                                    
                                    if (completed === selectedPermissions.length) {
                                        if (errors.length === selectedPermissions.length) {
                                            showRolePermissionsModalAlert('Tüm yetkiler güncellenirken hata oluştu!', 'error');
    } else {
                                            showRolePermissionsModalAlert(`${completed - errors.length} yetki güncellendi, ${errors.length} hata oluştu`, 'warning');
                                        }
                                        loadRoleMenuPermissions(roleId);
                                        loadAvailableMenusForRolePermission(roleId);
                                    }
                                }
                            });
                        });
            } else {
                        showRolePermissionsModalAlert('Tüm yetkiler kaldırıldı!', 'success');
                        loadRoleMenuPermissions(roleId);
                        loadAvailableMenusForRolePermission(roleId);
                    }
                } else {
                    showRolePermissionsModalAlert('Yetkiler güncellenirken hata oluştu!', 'error');
            }
        },
        error: function() {
                showRolePermissionsModalAlert('Yetkiler güncellenirken hata oluştu!', 'error');
            }
        });
    };

    // Role Details Modal
    function showRoleDetails(role) {
        const modalHtml = `
            <div class="modal fade" id="roleDetailsModal" tabindex="-1">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Rol Detayları</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <div class="row">
                                <div class="col-md-6">
                                    <h6>Rol Bilgileri</h6>
                                    <p><strong>ID:</strong> ${role.Id}</p>
                                    <p><strong>Rol Adı:</strong> ${role.Name}</p>
                                    <p><strong>Açıklama:</strong> ${role.Description || '-'}</p>
                                    <p><strong>Durum:</strong> <span class="badge ${role.IsActive ? 'bg-success' : 'bg-danger'}">${role.IsActive ? 'Aktif' : 'Pasif'}</span></p>
                    </div>
                                <div class="col-md-6">
                                    <h6>Zaman Bilgileri</h6>
                                    <p><strong>Oluşturulma:</strong> ${role.CreatedDate ? new Date(role.CreatedDate).toLocaleString('tr-TR') : '-'}</p>
                                    <p><strong>Son Güncelleme:</strong> ${role.UpdatedDate ? new Date(role.UpdatedDate).toLocaleString('tr-TR') : '-'}</p>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Kapat</button>
                        </div>
                    </div>
                    </div>
                </div>
            `;
        
        // Remove existing modal if any
        $('#roleDetailsModal').remove();
        
        // Add new modal to body
        $('body').append(modalHtml);
        
        // Show modal
        $('#roleDetailsModal').modal('show');
        
        // Remove modal from DOM when hidden
        $('#roleDetailsModal').on('hidden.bs.modal', function() {
            $(this).remove();
        });
    }

    // Modal Alert Functions
    function showRolePermissionsModalAlert(message, type) {
        var alertClass = type === 'success' ? 'alert-success' : 
                         (type === 'info' ? 'alert-info' : 
                         (type === 'warning' ? 'alert-warning' : 'alert-danger'));
        $('#rolePermissionsModalAlert')
            .removeClass('alert-success alert-danger alert-info alert-warning')
        .addClass(alertClass)
            .find('#rolePermissionsModalAlertMessage')
        .text(message);
        $('#rolePermissionsModalAlert').show();
    
    // 3 saniye sonra otomatik gizle
    setTimeout(function() {
            $('#rolePermissionsModalAlert').fadeOut();
    }, 3000);
}

    // Helper function for permission badge classes
    function getPermissionBadgeClass(permissionLevel) {
        switch(permissionLevel) {
            case 'VIEW': return 'bg-primary';
            case 'CREATE': return 'bg-success';
            case 'EDIT': return 'bg-warning text-dark';
            case 'UPDATE': return 'bg-warning text-dark';
            case 'DELETE': return 'bg-danger';
            default: return 'bg-secondary';
        }
    }

    // Initialize
    initDataGrid();
}

// Global Functions for Role Management
function openRolePermissionsModal(roleId, roleName, roleDescription, isActive) {
    $('#rolePermissionsModal').data('role-id', roleId);
    $('#modalRolePermissionsName').text(roleName);
    $('#modalRolePermissionsDescription').text(roleDescription);
    
    loadRoleMenuPermissions(roleId);
    loadAvailableMenusForRolePermission(roleId);
    $('#rolePermissionsModal').modal('show');
}

// Global Functions for Role Menu Permission Management
function loadRoleMenuPermissions(roleId) {
    console.log('Global loadRoleMenuPermissions for roleId:', roleId, 'Type:', typeof roleId);
    
    // Ensure roleId is a clean integer
    const cleanRoleId = parseInt(roleId);
    console.log('Global clean roleId:', cleanRoleId);
    
    $.ajax({
        url: '/Role/GetRoleMenuPermissions',
        type: 'GET',
        data: { roleId: cleanRoleId },
        success: function(response) {
            console.log('Global role menu permissions response:', response);
            if (response.success) {
                // Remove duplicates based on Id
                const uniquePermissions = response.data.filter((permission, index, self) => 
                    index === self.findIndex(p => p.Id === permission.Id)
                );
                console.log('Global unique permissions after deduplication:', uniquePermissions);
                displayAssignedRoleMenuPermissions(uniquePermissions);
            } else {
                showRolePermissionsModalAlert(response.message || response.error, 'error');
            }
        },
        error: function(xhr) {
            console.error('Global error loading role menu permissions:', xhr);
            showRolePermissionsModalAlert('Menü yetkileri yüklenirken hata oluştu!', 'error');
        }
    });
}

function loadAvailableMenusForRolePermission(roleId, search = '') {
    console.log('Global loadAvailableMenusForRolePermission for roleId:', roleId, 'search:', search);
    
    // Ensure roleId is a clean integer
    const cleanRoleId = parseInt(roleId);
    
    $.ajax({
        url: '/Role/GetAvailableMenusForRolePermission',
        type: 'GET',
        data: { roleId: cleanRoleId, search: search },
        success: function(response) {
            console.log('Global available menus response:', response);
            if (response.success) {
                // Remove duplicates based on Id
                const uniqueMenus = response.data.filter((menu, index, self) => 
                    index === self.findIndex(m => m.Id === menu.Id)
                );
                console.log('Global unique menus after deduplication:', uniqueMenus);
                displayAvailableMenusForRolePermission(uniqueMenus);
            } else {
                showRolePermissionsModalAlert(response.message || response.error, 'error');
            }
        },
        error: function(xhr) {
            console.error('Global error loading available menus:', xhr);
            showRolePermissionsModalAlert('Menüler yüklenirken hata oluştu!', 'error');
        }
    });
}

function displayAssignedRoleMenuPermissions(permissions) {
    var html = '';
    if (permissions.length === 0) {
        html = `
            <div class="empty-state text-center py-4">
                <i class="bi bi-gear fs-1 text-muted"></i>
                <p class="mb-0 text-muted small">Bu role atanmış menü yetkisi bulunmuyor</p>
            </div>
        `;
    } else {
        // Menülere göre grupla
        const groupedByMenu = {};
        permissions.forEach(function(permission) {
            const menuId = permission.MenuId;
            if (!groupedByMenu[menuId]) {
                groupedByMenu[menuId] = {
                    menuName: permission.MenuName || 'Bilinmeyen Menü',
                    menuIcon: permission.MenuIcon || 'bi bi-circle',
                    menuController: permission.MenuController || '',
                    menuAction: permission.MenuAction || '',
                    permissions: []
                };
            }
            groupedByMenu[menuId].permissions.push({
                id: permission.Id,
                permissionId: permission.PermissionId,
                level: permission.PermissionLevel,
                name: permission.PermissionName,
                notes: permission.Notes
            });
        });
        
        // Her menü için kart oluştur
        for (const menuId in groupedByMenu) {
            const menuData = groupedByMenu[menuId];
            const permBadges = menuData.permissions.map(p => 
                `<span class="badge ${getPermissionBadgeClass(p.level)} me-1 small">${p.level}</span>`
            ).join('');
            
            html += `
                <div class="card mb-2 border-0 shadow-sm">
                    <div class="card-body p-2">
                        <div class="d-flex justify-content-between align-items-start">
                            <div class="flex-grow-1">
                        <div class="d-flex align-items-center mb-1">
                                    <i class="${menuData.menuIcon} text-primary me-2"></i>
                                    <span class="fw-bold small">${menuData.menuName}</span>
                        </div>
                                <div class="text-muted" style="font-size: 10px;">${menuData.menuController}/${menuData.menuAction}</div>
                                <div class="mt-1">${permBadges}</div>
                    </div>
                            <div class="ms-2">
                                <button class="btn btn-sm btn-outline-warning edit-role-menu-permissions me-1" 
                                        data-menu-id="${menuId}"
                                        data-menu-name="${menuData.menuName}"
                                        title="Yetkileri Düzenle"
                                        type="button">
                                    <i class="bi bi-pencil"></i>
                                </button>
                                <button class="btn btn-sm btn-outline-danger remove-all-role-menu-permissions" 
                                        data-menu-id="${menuId}"
                                        title="Tüm Yetkileri Kaldır"
                                        type="button">
                                    <i class="bi bi-trash"></i>
                    </button>
                            </div>
                        </div>
                    </div>
                </div>
            `;
        }
    }
    $('#assignedRoleMenuPermissionsList').html(html);
}

function displayAvailableMenusForRolePermission(menus) {
    var html = '';
    if (menus.length === 0) {
        html = '<div class="text-muted text-center py-3 small">Eklenecek menü bulunamadı</div>';
    } else {
        menus.forEach(function(menu) {
            html += `
                <div class="d-flex justify-content-between align-items-center p-2 border rounded mb-1 bg-white">
                    <div class="flex-grow-1">
                        <div class="fw-bold d-flex align-items-center small">
                            <i class="${menu.Icon || 'bi bi-circle'} me-2 text-primary"></i>
                            ${menu.Name}
                        </div>
                        <small class="text-muted" style="font-size: 11px;">${menu.Controller}/${menu.Action || ''}</small>
                    </div>
                    <div class="ms-2">
                        <button class="btn btn-sm btn-success add-role-menu-permission" 
                                data-menu-id="${menu.Id}" 
                                data-menu-name="${menu.Name}"
                                title="Menü ve Yetki Ekle">
                            <i class="bi bi-plus-circle me-1"></i>
                            Ekle
                    </button>
                    </div>
                </div>
            `;
        });
    }
    $('#availableMenusForRolePermissionList').html(html);
}

function showRolePermissionsModalAlert(message, type) {
    var alertClass = type === 'success' ? 'alert-success' : 
                     (type === 'info' ? 'alert-info' : 
                     (type === 'warning' ? 'alert-warning' : 'alert-danger'));
    $('#rolePermissionsModalAlert')
        .removeClass('alert-success alert-danger alert-info alert-warning')
        .addClass(alertClass)
        .find('#rolePermissionsModalAlertMessage')
        .text(message);
    $('#rolePermissionsModalAlert').show();
    
    // 3 saniye sonra otomatik gizle
    setTimeout(function() {
        $('#rolePermissionsModalAlert').fadeOut();
    }, 3000);
}

function getPermissionBadgeClass(permissionLevel) {
    switch(permissionLevel) {
        case 'VIEW': return 'bg-primary';
        case 'CREATE': return 'bg-success';
        case 'EDIT': return 'bg-warning text-dark';
        case 'UPDATE': return 'bg-warning text-dark';
        case 'DELETE': return 'bg-danger';
        default: return 'bg-secondary';
    }
}

// Responsive Design Enhancements
$(window).resize(function() {
    // Adjust modal sizes for mobile
    if ($(window).width() < 768) {
        $('.modal-dialog').removeClass('modal-lg modal-xl').addClass('modal-sm');
    } else if ($(window).width() < 992) {
        $('.modal-dialog').removeClass('modal-xl').addClass('modal-lg');
    } else {
        $('.modal-dialog').removeClass('modal-sm modal-lg').addClass('modal-xl');
    }
});

// Initialize responsive behavior on page load
$(document).ready(function() {
    $(window).trigger('resize');
});