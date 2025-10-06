// Permission Management JavaScript

$(document).ready(function() {
    initializePermissionManagement();
});

function initializePermissionManagement() {
    let permissionsTable;
    let currentPermissionId = null;
    let isEditMode = false;

    // Initialize DataTable
    function initDataTable() {
        permissionsTable = $('#permissionsTable').DataTable({
            processing: true,
            serverSide: false,
            ajax: {
                url: '/Permission/GetPermissions',
                type: 'GET',
                dataSrc: function(json) {
                return json.data;
            },
                error: function(xhr, error, thrown) {
                console.error('DataTables AJAX error:', error, thrown);
            }
        },
            columns: [
                { data: 'id' },
                { data: 'name' },
                { data: 'code' },
                { data: 'description' },
                { 
                    data: 'isActive',
                    render: function(data, type, row) {
                        if (data) {
                            return '<span class="badge bg-success">Aktif</span>';
                        } else {
                            return '<span class="badge bg-danger">Pasif</span>';
                        }
                    }
                },
                { data: 'createdDate' },
                {
                    data: null,
                    orderable: false,
                    render: function(data, type, row) {
                        return '<button class="btn btn-sm btn-outline-primary view-roles" data-id="' + row.id + '" title="Rolleri Görüntüle">' +
                            '<i class="bi bi-shield-check"></i>' +
                            '</button>';
                    }
                },
                {
                    data: null,
                    orderable: false,
                    render: function(data, type, row) {
                        return '<button class="btn btn-sm btn-outline-warning view-users" data-id="' + row.id + '" title="Kullanıcıları Görüntüle">' +
                            '<i class="bi bi-people"></i>' +
                            '</button>';
                    }
                },
                {
                    data: null,
                    render: function(data, type, row) {
                        var actions = '';
                        
                        if (window.permissionPermissions && window.permissionPermissions.canView) {
                            actions += '<button class="btn btn-sm btn-outline-info me-1 view-permission" data-id="' + row.id + '" title="Görüntüle">';
                            actions += '<i class="bi bi-eye"></i></button>';
                        }
                        
                        if (window.permissionPermissions && window.permissionPermissions.canEdit) {
                            actions += '<button class="btn btn-sm btn-outline-primary me-1 edit-permission" data-id="' + row.id + '" title="Düzenle">';
                            actions += '<i class="bi bi-pencil"></i></button>';
                        }
                        
                        if (window.permissionPermissions && window.permissionPermissions.canDelete) {
                            actions += '<button class="btn btn-sm btn-outline-danger me-1 delete-permission" data-id="' + row.id + '" title="Sil">';
                            actions += '<i class="bi bi-trash"></i></button>';
                        }
                        
                        return actions;
                    }
                }
            ],
            language: {
                url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/tr.json'
            },
            responsive: true,
            pageLength: 10,
            order: [[0, 'desc']]
        });
    }

    // Add Permission Button
    $('#addPermissionBtn').click(function() {
        if (!window.permissionPermissions || !window.permissionPermissions.canCreate) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        isEditMode = false;
        $('#permissionModalLabel').text('Yeni Yetki');
        $('#permissionForm')[0].reset();
        $('#permissionId').val('');
        showSlideModal();
    });

    // Edit Permission Button
    $(document).on('click', '.edit-permission', function() {
        if (!window.permissionPermissions || !window.permissionPermissions.canEdit) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        const permissionId = $(this).data('id');
        isEditMode = true;
        currentPermissionId = permissionId;
        
        $.get('/Permission/GetPermission/' + permissionId)
            .done(function(response) {
                if (response.success && response.data) {
                    $('#permissionModalLabel').text('Yetki Düzenle');
                    $('#permissionId').val(response.data.id);
                    $('#permissionName').val(response.data.name);
                    $('#permissionCode').val(response.data.code);
                    $('#permissionDescription').val(response.data.description);
                    $('#permissionIsActive').prop('checked', response.data.isActive);
                    showSlideModal();
                } else {
                    showAlert('Yetki bilgileri alınamadı', 'error');
                }
            })
            .fail(function() {
                showAlert('Yetki bilgileri alınamadı', 'error');
            });
    });

    // Delete Permission Button
    $(document).on('click', '.delete-permission', function() {
        if (!window.permissionPermissions || !window.permissionPermissions.canDelete) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        currentPermissionId = $(this).data('id');
        if (confirm('Bu yetkiyi silmek istediğinizden emin misiniz?')) {
            $.ajax({
                url: '/Permission/Delete/' + currentPermissionId,
                type: 'DELETE',
                success: function(response) {
                    if (response.success) {
                        permissionsTable.ajax.reload();
                        showAlert(response.message, 'success');
                    } else {
                        showAlert(response.error, 'error');
                    }
                },
                error: function(xhr) {
                    const response = xhr.responseJSON;
                    showAlert(response ? response.error : 'Bir hata oluştu', 'error');
                }
            });
        }
    });

    // Save Permission
    $('#savePermissionBtn').click(function() {
        if (validateForm()) {
            const formData = {
                Name: $('#permissionName').val(),
                Code: $('#permissionCode').val(),
                Description: $('#permissionDescription').val(),
                IsActive: $('#permissionIsActive').is(':checked')
            };

            if (isEditMode) {
                formData.Id = parseInt($('#permissionId').val());
            }

            const url = isEditMode ? '/Permission/Update' : '/Permission/Create';
            const method = 'POST';

            $.ajax({
                url: url,
                type: method,
                contentType: 'application/json',
                data: JSON.stringify(formData),
                success: function(response) {
                    if (response.success) {
                        hideSlideModal();
                        permissionsTable.ajax.reload();
                        showAlert(response.message, 'success');
                    } else {
                        showAlert(response.error, 'error');
                    }
                },
                error: function(xhr) {
                    const response = xhr.responseJSON;
                    showAlert(response ? response.error : 'Bir hata oluştu', 'error');
                }
            });
        }
    });

    // Form Validation
    function validateForm() {
        let isValid = true;
        
        // Clear previous validation
        $('.form-control').removeClass('is-invalid');
        $('.invalid-feedback').text('');

        // Required fields
        const requiredFields = ['permissionName', 'permissionCode'];

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
        const modal = $('#permissionModal');
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
        const modal = $('#permissionModal');
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
        if (e.key === 'Escape' && $('#permissionModal').hasClass('show')) {
            hideSlideModal();
        }
    });
    
    // Close modal on close button
    $(document).on('click', '.btn-close', function() {
        hideSlideModal();
    });
    
    // Close modal on cancel button
    $(document).on('click', '#cancelPermissionBtn', function() {
        hideSlideModal();
    });

    function showAlert(message, type) {
        const alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
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

    // Permission Role Management
    $(document).on('click', '.view-roles', function() {
        var permissionId = $(this).data('id');
        openPermissionRoleModal(permissionId);
    });

    function openPermissionRoleModal(permissionId) {
        $('#permissionRoleModal').modal('show');
        
        // Store current permission ID
        $('#permissionRoleModal').data('permission-id', permissionId);
        
        // Show loading states
        showLoadingStates();
        
        // Load data in parallel for better performance
        Promise.all([
            loadPermissionInfoAsync(permissionId),
            loadPermissionRolesAsync(permissionId),
            loadAvailableRolesForPermissionAsync(permissionId)
        ]).then(() => {
            hideLoadingStates();
        }).catch((error) => {
            hideLoadingStates();
            showAlert('Veriler yüklenirken hata oluştu!', 'error');
        });
    }

    // Loading state functions
    function showLoadingStates() {
        $('#assignedPermissionRolesList').html(`
            <div class="text-center py-4">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Yükleniyor...</span>
                </div>
                <p class="mt-2 text-muted">Roller yükleniyor...</p>
            </div>
        `);
        
        $('#availablePermissionRolesList').html(`
            <div class="text-center py-4">
                <div class="spinner-border text-success" role="status">
                    <span class="visually-hidden">Yükleniyor...</span>
                </div>
                <p class="mt-2 text-muted">Mevcut roller yükleniyor...</p>
            </div>
        `);
    }
    
    function hideLoadingStates() {
        // Loading states will be replaced by actual data
    }

    function loadPermissionInfo(permissionId) {
        $.ajax({
            url: '/Permission/GetPermission',
            type: 'GET',
            data: { id: permissionId },
            success: function(response) {
                if (response.success) {
                    displayPermissionInfo(response.data);
                } else {
                    showPermissionRoleModalAlert(response.error, 'error');
                }
            },
            error: function() {
                showPermissionRoleModalAlert('Yetki bilgileri yüklenirken hata oluştu!', 'error');
            }
        });
    }

    // Async versions for parallel loading
    function loadPermissionInfoAsync(permissionId) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: '/Permission/GetPermission',
                type: 'GET',
                data: { id: permissionId },
                success: function(response) {
                    if (response.success) {
                        displayPermissionInfo(response.data);
                        resolve(response);
                    } else {
                        showPermissionRoleModalAlert(response.error, 'error');
                        reject(response.error);
                    }
                },
                error: function() {
                    showPermissionRoleModalAlert('Yetki bilgileri yüklenirken hata oluştu!', 'error');
                    reject('Yetki bilgileri yüklenirken hata oluştu!');
                }
            });
        });
    }

    function displayPermissionInfo(permission) {
        $('#modalPermissionRoleName').text(permission.name || '-');
        $('#modalPermissionRoleCode').text(permission.code || '-');
        
        const statusBadge = permission.isActive ? 
            '<span class="badge bg-success">Aktif</span>' : 
            '<span class="badge bg-danger">Pasif</span>';
        $('#modalPermissionRoleStatus').html(statusBadge);
        
        // User modalı için de aynı bilgileri göster
        displayPermissionUserInfo(permission);
    }

    function displayPermissionUserInfo(permission) {
        $('#modalPermissionUserName').text(permission.name || '-');
        $('#modalPermissionUserCode').text(permission.code || '-');
        
        const statusBadge = permission.isActive ? 
            '<span class="badge bg-success">Aktif</span>' : 
            '<span class="badge bg-danger">Pasif</span>';
        $('#modalPermissionUserStatus').html(statusBadge);
    }

    function showPermissionRoleModalAlert(message, type) {
        const alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
        const alertHtml = `
            <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
                <i class="bi bi-${type === 'success' ? 'check-circle' : 'exclamation-triangle'} me-2"></i>
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `;
        
        // Remove existing alerts
        $('#permissionRoleModalAlert').remove();
        
        // Add new alert
        $('#permissionRoleModal .modal-body').prepend(alertHtml);
        
        // Auto remove after 5 seconds
        setTimeout(function() {
            $('#permissionRoleModal .alert').fadeOut();
        }, 5000);
    }

    function loadPermissionRoles(permissionId) {
        $.ajax({
            url: '/Permission/GetRolesByPermission',
            type: 'GET',
            data: { permissionId: permissionId },
            success: function(response) {
                if (response.success) {
                    displayAssignedPermissionRoles(response.data);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Roller yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function loadPermissionRolesAsync(permissionId) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: '/Permission/GetRolesByPermission',
                type: 'GET',
                data: { permissionId: permissionId },
                success: function(response) {
                    if (response.success) {
                        displayAssignedPermissionRoles(response.data);
                        resolve(response);
                    } else {
                        showAlert(response.error, 'error');
                        reject(response.error);
                    }
                },
                error: function() {
                    showAlert('Roller yüklenirken hata oluştu!', 'error');
                    reject('Roller yüklenirken hata oluştu!');
                }
            });
        });
    }

    function loadAvailableRolesForPermission(permissionId, search = '') {
        // Bu fonksiyon bir permission'a atanabilecek rolleri getirmek için kullanılacak
        // Şu an için tüm rolleri getiriyoruz
        $.ajax({
            url: '/Role/GetRoles',
            type: 'GET',
            success: function(response) {
                if (response.data) {
                    // Arama filtresi uygula
                    let filteredRoles = response.data;
                    if (search) {
                        filteredRoles = response.data.filter(role => 
                            role.name.toLowerCase().includes(search.toLowerCase()) ||
                            (role.description && role.description.toLowerCase().includes(search.toLowerCase()))
                        );
                    }
                    displayAvailablePermissionRoles(filteredRoles);
                } else {
                    showAlert('Roller yüklenirken hata oluştu!', 'error');
                }
            },
            error: function() {
                showAlert('Roller yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function loadAvailableRolesForPermissionAsync(permissionId, search = '') {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: '/Role/GetRoles',
                type: 'GET',
                success: function(response) {
                    if (response.data) {
                        // Arama filtresi uygula
                        let filteredRoles = response.data;
                        if (search) {
                            filteredRoles = response.data.filter(role => 
                                role.name.toLowerCase().includes(search.toLowerCase()) ||
                                (role.description && role.description.toLowerCase().includes(search.toLowerCase()))
                            );
                        }
                        displayAvailablePermissionRoles(filteredRoles);
                        resolve(response);
                    } else {
                        showAlert('Roller yüklenirken hata oluştu!', 'error');
                        reject('Roller yüklenirken hata oluştu!');
                    }
                },
                error: function() {
                    showAlert('Roller yüklenirken hata oluştu!', 'error');
                    reject('Roller yüklenirken hata oluştu!');
                }
            });
        });
    }

    function displayAssignedPermissionRoles(roles) {
        var html = '';
        if (roles.length === 0) {
            html = `
                <div class="empty-state">
                    <i class="bi bi-shield-x"></i>
                    <p class="mb-0">Bu yetkiye atanmış rol bulunmuyor</p>
                </div>
            `;
        } else {
            roles.forEach(function(role) {
                html += `
                    <div class="role-item d-flex justify-content-between align-items-center p-3 bg-light border rounded">
                        <div class="role-info">
                            <div class="d-flex align-items-center mb-1">
                                <i class="bi bi-shield-check text-primary me-2"></i>
                                <span class="fw-bold">${role.name}</span>
                                <span class="role-badge bg-primary text-white ms-2">Aktif</span>
                            </div>
                            <div class="role-description">${role.description || 'Açıklama bulunmuyor'}</div>
                        </div>
                        <div class="role-actions">
                            <button class="btn btn-sm btn-outline-danger remove-permission-role" data-role-id="${role.id}" title="Rolü Kaldır">
                                <i class="bi bi-x"></i>
                            </button>
                        </div>
                    </div>
                `;
            });
        }
        $('#assignedPermissionRolesList').html(html);
    }

    function displayAvailablePermissionRoles(roles) {
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
                html += `
                    <div class="role-item d-flex justify-content-between align-items-center p-3 bg-white border rounded">
                        <div class="role-info">
                            <div class="d-flex align-items-center mb-1">
                                <i class="bi bi-shield text-success me-2"></i>
                                <span class="fw-bold">${role.name}</span>
                            </div>
                            <div class="role-description">${role.description || 'Açıklama bulunmuyor'}</div>
                        </div>
                        <div class="role-actions">
                            <button class="btn btn-sm btn-outline-success add-permission-role" data-role-id="${role.id}" title="Role Ekle">
                                <i class="bi bi-plus"></i>
                            </button>
                        </div>
                    </div>
                `;
            });
        }
        $('#availablePermissionRolesList').html(html);
    }

    // Permission User Management
    $(document).on('click', '.view-users', function() {
        var permissionId = $(this).data('id');
        openPermissionUserModal(permissionId);
    });

    function openPermissionUserModal(permissionId) {
        $('#permissionUserModal').modal('show');
        
        // Store current permission ID
        $('#permissionUserModal').data('permission-id', permissionId);
        
        // Show loading states
        showUserLoadingStates();
        
        // Load data in parallel for better performance
        Promise.all([
            loadPermissionInfoAsync(permissionId),
            loadPermissionUsersAsync(permissionId),
            loadAvailableUsersForPermissionAsync(permissionId)
        ]).then(() => {
            hideUserLoadingStates();
        }).catch((error) => {
            hideUserLoadingStates();
            showAlert('Veriler yüklenirken hata oluştu!', 'error');
        });
    }

    // User loading state functions
    function showUserLoadingStates() {
        $('#assignedPermissionUsersList').html(`
            <div class="text-center py-4">
                <div class="spinner-border text-warning" role="status">
                    <span class="visually-hidden">Yükleniyor...</span>
                </div>
                <p class="mt-2 text-muted">Kullanıcılar yükleniyor...</p>
            </div>
        `);
        
        $('#availablePermissionUsersList').html(`
            <div class="text-center py-4">
                <div class="spinner-border text-success" role="status">
                    <span class="visually-hidden">Yükleniyor...</span>
                </div>
                <p class="mt-2 text-muted">Mevcut kullanıcılar yükleniyor...</p>
            </div>
        `);
    }
    
    function hideUserLoadingStates() {
        // Loading states will be replaced by actual data
    }

    function showPermissionUserModalAlert(message, type) {
        const alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
        const alertHtml = `
            <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
                <i class="bi bi-${type === 'success' ? 'check-circle' : 'exclamation-triangle'} me-2"></i>
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `;
        
        // Remove existing alerts
        $('#permissionUserModalAlert').remove();
        
        // Add new alert
        $('#permissionUserModal .modal-body').prepend(alertHtml);
        
        // Auto remove after 5 seconds
        setTimeout(function() {
            $('#permissionUserModal .alert').fadeOut();
        }, 5000);
    }

    function loadPermissionUsers(permissionId) {
        $.ajax({
            url: '/Permission/GetUsersByPermission',
            type: 'GET',
            data: { permissionId: permissionId },
            success: function(response) {
                if (response.success) {
                    displayAssignedPermissionUsers(response.data);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Kullanıcılar yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function loadPermissionUsersAsync(permissionId) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: '/Permission/GetUsersByPermission',
                type: 'GET',
                data: { permissionId: permissionId },
                success: function(response) {
                    if (response.success) {
                        displayAssignedPermissionUsers(response.data);
                        resolve(response);
                    } else {
                        showAlert(response.error, 'error');
                        reject(response.error);
                    }
                },
                error: function() {
                    showAlert('Kullanıcılar yüklenirken hata oluştu!', 'error');
                    reject('Kullanıcılar yüklenirken hata oluştu!');
                }
            });
        });
    }

    function loadAvailableUsersForPermission(permissionId, search = '') {
        // Bu fonksiyon bir permission'a atanabilecek kullanıcıları getirmek için kullanılacak
        // Şu an için tüm kullanıcıları getiriyoruz
        $.ajax({
            url: '/User/GetUsers',
            type: 'GET',
            success: function(response) {
                if (response.data) {
                    // Arama filtresi uygula
                    let filteredUsers = response.data;
                    if (search) {
                        filteredUsers = response.data.filter(user => 
                            user.username.toLowerCase().includes(search.toLowerCase()) ||
                            user.firstName.toLowerCase().includes(search.toLowerCase()) ||
                            user.lastName.toLowerCase().includes(search.toLowerCase()) ||
                            user.email.toLowerCase().includes(search.toLowerCase())
                        );
                    }
                    displayAvailablePermissionUsers(filteredUsers);
                } else {
                    showAlert('Kullanıcılar yüklenirken hata oluştu!', 'error');
                }
            },
            error: function() {
                showAlert('Kullanıcılar yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function loadAvailableUsersForPermissionAsync(permissionId, search = '') {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: '/User/GetUsers',
                type: 'GET',
                success: function(response) {
                    if (response.data) {
                        // Arama filtresi uygula
                        let filteredUsers = response.data;
                        if (search) {
                            filteredUsers = response.data.filter(user => 
                                user.username.toLowerCase().includes(search.toLowerCase()) ||
                                user.firstName.toLowerCase().includes(search.toLowerCase()) ||
                                user.lastName.toLowerCase().includes(search.toLowerCase()) ||
                                user.email.toLowerCase().includes(search.toLowerCase())
                            );
                        }
                        displayAvailablePermissionUsers(filteredUsers);
                        resolve(response);
                    } else {
                        showAlert('Kullanıcılar yüklenirken hata oluştu!', 'error');
                        reject('Kullanıcılar yüklenirken hata oluştu!');
                    }
                },
                error: function() {
                    showAlert('Kullanıcılar yüklenirken hata oluştu!', 'error');
                    reject('Kullanıcılar yüklenirken hata oluştu!');
                }
            });
        });
    }

    function displayAssignedPermissionUsers(users) {
        var html = '';
        if (users.length === 0) {
            html = `
                <div class="empty-state">
                    <i class="bi bi-people"></i>
                    <p class="mb-0">Bu yetkiye atanmış kullanıcı bulunmuyor</p>
                </div>
            `;
        } else {
            users.forEach(function(user) {
                var fullName = (user.firstName || '') + ' ' + (user.lastName || '');
                var statusBadge = user.isActive ? 
                    '<span class="badge bg-success ms-2">Aktif</span>' : 
                    '<span class="badge bg-danger ms-2">Pasif</span>';
                
                html += `
                    <div class="role-item d-flex justify-content-between align-items-center p-3 bg-light border rounded">
                        <div class="role-info">
                            <div class="d-flex align-items-center mb-1">
                                <i class="bi bi-person-check text-warning me-2"></i>
                                <span class="fw-bold">${user.username}</span>
                                ${statusBadge}
                            </div>
                            <div class="role-description">${fullName.trim() || 'Ad Soyad bulunmuyor'} - ${user.email}</div>
                        </div>
                        <div class="role-actions">
                            <button class="btn btn-sm btn-outline-danger remove-permission-user" data-user-id="${user.id}" title="Kullanıcıyı Kaldır">
                                <i class="bi bi-x"></i>
                            </button>
                        </div>
                    </div>
                `;
            });
        }
        $('#assignedPermissionUsersList').html(html);
    }

    function displayAvailablePermissionUsers(users) {
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
                var fullName = (user.firstName || '') + ' ' + (user.lastName || '');
                html += `
                    <div class="role-item d-flex justify-content-between align-items-center p-3 bg-white border rounded">
                        <div class="role-info">
                            <div class="d-flex align-items-center mb-1">
                                <i class="bi bi-person text-success me-2"></i>
                                <span class="fw-bold">${user.username}</span>
                            </div>
                            <div class="role-description">${fullName.trim() || 'Ad Soyad bulunmuyor'} - ${user.email}</div>
                        </div>
                        <div class="role-actions">
                            <button class="btn btn-sm btn-outline-success add-permission-user" data-user-id="${user.id}" title="Kullanıcıya Ekle">
                                <i class="bi bi-plus"></i>
                            </button>
                        </div>
                    </div>
                `;
            });
        }
        $('#availablePermissionUsersList').html(html);
    }

    // Role Actions
    $(document).on('click', '.add-permission-role', function() {
        var roleId = $(this).data('role-id');
        var permissionId = $('#permissionRoleModal').data('permission-id');
        var $button = $(this);
        
        // Show loading state
        $button.html('<span class="loading-spinner"></span>').prop('disabled', true);

        $.ajax({
            url: '/Permission/AssignPermissionToRole',
            type: 'POST',
            data: { roleId: roleId, permissionId: permissionId },
            success: function(response) {
                if (response.success) {
                    showPermissionRoleModalAlert(response.message, 'success');
                    loadPermissionRoles(permissionId);
                    loadAvailableRolesForPermission(permissionId);
                } else {
                    showPermissionRoleModalAlert(response.error, 'error');
                }
            },
            error: function() {
                showPermissionRoleModalAlert('Rol atanırken hata oluştu!', 'error');
            },
            complete: function() {
                // Reset button state
                $button.html('<i class="bi bi-plus"></i>').prop('disabled', false);
            }
        });
    });

    $(document).on('click', '.remove-permission-role', function() {
        var roleId = $(this).data('role-id');
        var permissionId = $('#permissionRoleModal').data('permission-id');
        var $button = $(this);
        
        // Show loading state
        $button.html('<span class="loading-spinner"></span>').prop('disabled', true);
        
        $.ajax({
            url: '/Permission/RemovePermissionFromRole',
            type: 'DELETE',
            data: { roleId: roleId, permissionId: permissionId },
            success: function(response) {
                if (response.success) {
                    showPermissionRoleModalAlert(response.message, 'success');
                    loadPermissionRoles(permissionId);
                    loadAvailableRolesForPermission(permissionId);
                } else {
                    showPermissionRoleModalAlert(response.error, 'error');
                }
            },
            error: function() {
                showPermissionRoleModalAlert('Rol kaldırılırken hata oluştu!', 'error');
            },
            complete: function() {
                // Reset button state
                $button.html('<i class="bi bi-x"></i>').prop('disabled', false);
            }
        });
    });

    // User Actions
    $(document).on('click', '.add-permission-user', function() {
        var userId = $(this).data('user-id');
        var permissionId = $('#permissionUserModal').data('permission-id');
        var $button = $(this);
        
        // Show loading state
        $button.html('<span class="loading-spinner"></span>').prop('disabled', true);
        
        $.ajax({
            url: '/Permission/AssignPermissionToUser',
            type: 'POST',
            data: { userId: userId, permissionId: permissionId },
            success: function(response) {
                if (response.success) {
                    showPermissionUserModalAlert(response.message, 'success');
                    loadPermissionUsers(permissionId);
                    loadAvailableUsersForPermission(permissionId);
                } else {
                    showPermissionUserModalAlert(response.error, 'error');
                }
            },
            error: function() {
                showPermissionUserModalAlert('Kullanıcı atanırken hata oluştu!', 'error');
            },
            complete: function() {
                // Reset button state
                $button.html('<i class="bi bi-plus"></i>').prop('disabled', false);
            }
        });
    });

    $(document).on('click', '.remove-permission-user', function() {
        var userId = $(this).data('user-id');
        var permissionId = $('#permissionUserModal').data('permission-id');
        var $button = $(this);
        
        // Show loading state
        $button.html('<span class="loading-spinner"></span>').prop('disabled', true);
        
        $.ajax({
            url: '/Permission/RemovePermissionFromUser',
            type: 'DELETE',
            data: { userId: userId, permissionId: permissionId },
            success: function(response) {
                if (response.success) {
                    showPermissionUserModalAlert(response.message, 'success');
                    loadPermissionUsers(permissionId);
                    loadAvailableUsersForPermission(permissionId);
                } else {
                    showPermissionUserModalAlert(response.error, 'error');
                }
            },
            error: function() {
                showPermissionUserModalAlert('Kullanıcı kaldırılırken hata oluştu!', 'error');
            },
            complete: function() {
                // Reset button state
                $button.html('<i class="bi bi-x"></i>').prop('disabled', false);
            }
        });
    });

    // Search Functions
    $('#permissionRoleSearchInput').on('input', function() {
        var search = $(this).val();
        var permissionId = $('#permissionRoleModal').data('permission-id');
        loadAvailableRolesForPermission(permissionId, search);
    });

    $('#permissionUserSearchInput').on('input', function() {
        var search = $(this).val();
        var permissionId = $('#permissionUserModal').data('permission-id');
        loadAvailableUsersForPermission(permissionId, search);
    });

    // Initialize
    initDataTable();
}