// User Management JavaScript

$(document).ready(function() {
    initializeUserManagement();
});

function initializeUserManagement() {
    let usersTable;
    let currentUserId = null;
    let isEditMode = false;

    // Initialize DataTable
    function initDataTable() {
        usersTable = $('#usersTable').DataTable({
            processing: true,
            serverSide: false,
            ajax: {
                url: '/User/GetUsers',
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
                { data: 'username' },
                { 
                    data: null,
                    render: function(data, type, row) {
                        return row.firstName + ' ' + row.lastName;
                    }
                },
                { data: 'email' },
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
                {
                    data: null,
                    orderable: false,
                    render: function(data, type, row) {
                        return '<button class="btn btn-sm btn-outline-info view-roles" data-id="' + row.id + '" title="Rolleri Görüntüle">' +
                            '<i class="bi bi-shield-check"></i>' +
                            '</button>';
                    }
                },
                {
                    data: null,
                    orderable: false,
                    render: function(data, type, row) {
                        return '<button class="btn btn-sm btn-outline-warning view-menus" data-id="' + row.id + '" title="Menü Yetkilerini Görüntüle">' +
                            '<i class="bi bi-list-ul"></i>' +
                            '</button>';
                    }
                },
                { data: 'createdDate' },
                { data: 'lastLoginDate' },
                {
                    data: null,
                    render: function(data, type, row) {
                        var actions = '';
                        
                        if (window.userPermissions && window.userPermissions.canView) {
                            actions += '<button class="btn btn-sm btn-outline-info me-1 view-user" data-id="' + row.id + '" title="Görüntüle">';
                            actions += '<i class="bi bi-eye"></i></button>';
                        }
                        
                        if (window.userPermissions && window.userPermissions.canEdit) {
                            actions += '<button class="btn btn-sm btn-outline-primary me-1 edit-user" data-id="' + row.id + '" title="Düzenle">';
                            actions += '<i class="bi bi-pencil"></i></button>';
                        }
                        
                        if (window.userPermissions && window.userPermissions.canDelete) {
                            actions += '<button class="btn btn-sm btn-outline-danger me-1 delete-user" data-id="' + row.id + '" title="Sil">';
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

    // Add User Button
    $('#addUserBtn').click(function() {
        if (!window.userPermissions || !window.userPermissions.canCreate) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        isEditMode = false;
        $('#userModalLabel').text('Yeni Kullanıcı');
        $('#userForm')[0].reset();
        $('#userId').val('');
        $('#passwordHelp').show();
        $('#password').attr('required', true);
        $('#generatePasswordBtn').show();
        $('#resetPasswordBtn').hide();
        showSlideModal();
    });

    // Edit User Button
    $(document).on('click', '.edit-user', function() {
        if (!window.userPermissions || !window.userPermissions.canEdit) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        const userId = $(this).data('id');
        isEditMode = true;
        currentUserId = userId;
        
        $.get('/User/GetUser/' + userId)
            .done(function(response) {
                if (response.data) {
                    $('#userModalLabel').text('Kullanıcı Düzenle');
                    $('#userId').val(response.data.id);
                    $('#username').val(response.data.username);
                    $('#firstName').val(response.data.firstName);
                    $('#lastName').val(response.data.lastName);
                    $('#email').val(response.data.email);
                    $('#isActive').prop('checked', response.data.isActive);
                    $('#password').val('');
                    $('#passwordHelp').hide();
                    $('#password').removeAttr('required');
                    $('#generatePasswordBtn').hide();
                    $('#resetPasswordBtn').show();
                    showSlideModal();
                } else {
                    showAlert('Kullanıcı bilgileri alınamadı', 'error');
                }
            })
            .fail(function() {
                showAlert('Kullanıcı bilgileri alınamadı', 'error');
            });
    });

    // Delete User Button
    $(document).on('click', '.delete-user', function() {
        if (!window.userPermissions || !window.userPermissions.canDelete) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        currentUserId = $(this).data('id');
        $('#deleteModal').modal('show');
    });

    // Save User
    $('#saveUserBtn').click(function() {
        if (validateForm()) {
            const formData = {
                Username: $('#username').val(),
                FirstName: $('#firstName').val(),
                LastName: $('#lastName').val(),
                Email: $('#email').val(),
                Password: $('#password').val(),
                IsActive: $('#isActive').is(':checked')
            };

            const url = isEditMode ? '/User/Update/' + currentUserId : '/User/Create';
            const method = isEditMode ? 'PUT' : 'POST';

            $.ajax({
                url: url,
                type: method,
                contentType: 'application/json',
                data: JSON.stringify(formData),
                success: function(response) {
                    if (response.success) {
                        hideSlideModal();
                        usersTable.ajax.reload();
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

    // Confirm Delete
    $('#confirmDeleteBtn').click(function() {
        $.ajax({
            url: '/User/Delete/' + currentUserId,
            type: 'DELETE',
            success: function(response) {
                if (response.success) {
                    $('#deleteModal').modal('hide');
                    usersTable.ajax.reload();
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
    });

    // Form Validation
    function validateForm() {
        let isValid = true;
        
        // Clear previous validation
        $('.form-control').removeClass('is-invalid');
        $('.invalid-feedback').text('');

        // Required fields
        const requiredFields = ['username', 'firstName', 'lastName', 'email'];
        if (!isEditMode) {
            requiredFields.push('password');
        }

        requiredFields.forEach(function(field) {
            const value = $('#' + field).val().trim();
            if (!value) {
                $('#' + field).addClass('is-invalid');
                $('#' + field).siblings('.invalid-feedback').text('Bu alan gereklidir');
                isValid = false;
            }
        });

        // Email validation
        const email = $('#email').val();
        if (email && !isValidEmail(email)) {
            $('#email').addClass('is-invalid');
            $('#email').siblings('.invalid-feedback').text('Geçerli bir email adresi giriniz');
            isValid = false;
        }

        // Password validation (only for new users or when password is provided)
        const password = $('#password').val();
        if ((!isEditMode || password) && password.length < 6) {
            $('#password').addClass('is-invalid');
            $('#password').siblings('.invalid-feedback').text('Şifre en az 6 karakter olmalıdır');
            isValid = false;
        }

        return isValid;
    }

    function isValidEmail(email) {
        var atIndex = email.indexOf('@');
        var dotIndex = email.indexOf('.');
        return atIndex > 0 && dotIndex > atIndex;
    }

    // Slide Modal Functions
    function showSlideModal() {
        const modal = $('#userModal');
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
        const modal = $('#userModal');
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
        if (e.key === 'Escape' && $('#userModal').hasClass('show')) {
            hideSlideModal();
        }
    });
    
    // Close modal on close button
    $(document).on('click', '.btn-close', function() {
        hideSlideModal();
    });
    
    // Close modal on cancel button
    $(document).on('click', '#cancelUserBtn', function() {
        hideSlideModal();
    });
    
    // Generate Password Button
    $(document).on('click', '#generatePasswordBtn', function() {
        const generatedPassword = generateRandomPassword();
        $('#password').val(generatedPassword);
        showModalAlert('Otomatik şifre oluşturuldu: ' + generatedPassword, 'success');
    });
    
    // Reset Password Button
    $(document).on('click', '#resetPasswordBtn', function() {
        if (confirm('Kullanıcının şifresini sıfırlamak istediğinizden emin misiniz?')) {
            const newPassword = generateRandomPassword();
            $('#password').val(newPassword);
            showModalAlert('Şifre sıfırlandı. Yeni şifre: ' + newPassword, 'success');
        }
    });
    
    // Toggle Password Visibility
    $(document).on('click', '#togglePasswordBtn', function() {
        const passwordInput = $('#password');
        const toggleIcon = $('#togglePasswordIcon');
        
        if (passwordInput.attr('type') === 'password') {
            passwordInput.attr('type', 'text');
            toggleIcon.removeClass('bi-eye').addClass('bi-eye-slash');
        } else {
            passwordInput.attr('type', 'password');
            toggleIcon.removeClass('bi-eye-slash').addClass('bi-eye');
        }
    });

    // Generate Random Password
    function generateRandomPassword() {
        const length = 12;
        const charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
        let password = "";
        
        // Ensure at least one character from each category
        const lowercase = "abcdefghijklmnopqrstuvwxyz";
        const uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const numbers = "0123456789";
        const symbols = "!@#$%^&*";
        
        password += lowercase.charAt(Math.floor(Math.random() * lowercase.length));
        password += uppercase.charAt(Math.floor(Math.random() * uppercase.length));
        password += numbers.charAt(Math.floor(Math.random() * numbers.length));
        password += symbols.charAt(Math.floor(Math.random() * symbols.length));
        
        // Fill the rest randomly
        for (let i = 4; i < length; i++) {
            password += charset.charAt(Math.floor(Math.random() * charset.length));
        }
        
        // Shuffle the password
        return password.split('').sort(() => Math.random() - 0.5).join('');
    }

    // Modal Alert Function
    function showModalAlert(message, type) {
        const alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
        const alertHtml = '<div class="alert ' + alertClass + ' alert-dismissible fade show" role="alert">' +
            message +
            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
            '</div>';
        
        // Remove existing modal alerts
        $('#userModal .alert').remove();
        
        // Add new alert to modal body
        $('#userModal .modal-body').prepend(alertHtml);
        
        // Auto remove after 5 seconds
        setTimeout(function() {
            $('#userModal .alert').fadeOut();
        }, 5000);
    }

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

    // User Role Management
    $(document).on('click', '.view-roles', function() {
        var userId = $(this).data('id');
        openUserRoleModal(userId);
    });

    function openUserRoleModal(userId) {
        $('#userRoleModal').modal('show');
        loadUserInfo(userId);
        loadUserRoles(userId);
        loadAvailableRolesForUser(userId);
        
        // Store current user ID
        $('#userRoleModal').data('user-id', userId);
    }

    function loadUserInfo(userId) {
        $.ajax({
            url: '/User/GetUser',
            type: 'GET',
            data: { id: userId },
            success: function(response) {
                if (response.success) {
                    displayUserInfo(response.data);
                } else {
                    showRoleModalAlert(response.error, 'error');
                }
            },
            error: function() {
                showRoleModalAlert('Kullanıcı bilgileri yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function displayUserInfo(user) {
        $('#modalUserName').text(user.username || '-');
        // firstName ve lastName'i birleştir ve trim yap
        var fullName = ((user.firstName || '') + ' ' + (user.lastName || '')).trim();
        $('#modalUserFullName').text(fullName || '-');
        $('#modalUserEmail').text(user.email || '-');
        
        const statusBadge = user.isActive ? 
            '<span class="badge bg-success">Aktif</span>' : 
            '<span class="badge bg-danger">Pasif</span>';
        $('#modalUserStatus').html(statusBadge);
        
        // Menü modalı için de aynı bilgileri göster
        displayMenuUserInfo(user);
    }

    function showRoleModalAlert(message, type) {
        const alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
        const alertHtml = `
            <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
                <i class="bi bi-${type === 'success' ? 'check-circle' : 'exclamation-triangle'} me-2"></i>
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `;
        
        // Remove existing alerts
        $('#roleModalAlert').remove();
        
        // Add new alert
        $('#userRoleModal .modal-body').prepend(alertHtml);
        
        // Auto remove after 5 seconds
        setTimeout(function() {
            $('#userRoleModal .alert').fadeOut();
        }, 5000);
    }

    function loadUserRoles(userId) {
        $.ajax({
            url: '/User/GetUserRoles',
            type: 'GET',
            data: { userId: userId },
            success: function(response) {
                if (response.success) {
                    displayAssignedUserRoles(response.data);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Roller yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function loadAvailableRolesForUser(userId, search = '') {
        $.ajax({
            url: '/User/GetAvailableRolesForUser',
            type: 'GET',
            data: { userId: userId, search: search },
            success: function(response) {
                if (response.success) {
                    displayAvailableUserRoles(response.data);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Roller yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function displayAssignedUserRoles(roles) {
        var html = '';
        if (roles.length === 0) {
            html = `
                <div class="empty-state">
                    <i class="bi bi-shield-x"></i>
                    <p class="mb-0">Kullanıcının atanmış rolü bulunmuyor</p>
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
                            <button class="btn btn-sm btn-outline-danger remove-user-role" data-role-id="${role.id}" title="Rolü Kaldır">
                                <i class="bi bi-x"></i>
                            </button>
                        </div>
                    </div>
                `;
            });
        }
        $('#assignedUserRolesList').html(html);
    }

    function displayAvailableUserRoles(roles) {
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
                            <button class="btn btn-sm btn-outline-success add-user-role" data-role-id="${role.id}" title="Role Ekle">
                                <i class="bi bi-plus"></i>
                            </button>
                        </div>
                    </div>
                `;
            });
        }
        $('#availableUserRolesList').html(html);
    }

    // User Menu Permissions Management
    $(document).on('click', '.view-menus', function() {
        var userId = $(this).data('id');
        openUserMenuPermissionsModal(userId);
    });

    function openUserMenuPermissionsModal(userId) {
        $('#userMenuPermissionsModal').modal('show');
        
        // Kullanıcı bilgilerini yükle
        loadUserInfo(userId);
        
        // Menü yetkilerini yükle
        loadUserMenus(userId);
        loadAvailableMenusForUser(userId);
        
        // Store current user ID
        $('#userMenuPermissionsModal').data('user-id', userId);
    }

    function displayMenuUserInfo(user) {
        $('#modalMenuUserName').text(user.username || '-');
        // firstName ve lastName'i birleştir
        var fullName = ((user.firstName || '') + ' ' + (user.lastName || '')).trim();
        $('#modalMenuUserFullName').text(fullName || '-');
        $('#modalMenuUserEmail').text(user.email || '-');
        
        var statusBadge = user.isActive ? 
            '<span class="badge bg-success">Aktif</span>' : 
            '<span class="badge bg-danger">Pasif</span>';
        $('#modalMenuUserStatus').html(statusBadge);
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

    function loadUserMenus(userId) {
        $.ajax({
            url: '/User/GetUserMenus',
            type: 'GET',
            data: { userId: userId },
            success: function(response) {
                if (response.success) {
                    displayAssignedUserMenus(response.data);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Menü yetkileri yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function loadAvailableMenusForUser(userId, search = '') {
        $.ajax({
            url: '/User/GetAvailableMenusForUser',
            type: 'GET',
            data: { userId: userId, search: search },
            success: function(response) {
                if (response.success) {
                    displayAvailableUserMenus(response.data);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Menüler yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function displayAssignedUserMenus(menus) {
        var html = '';
        if (menus.length === 0) {
            html = `
                <div class="empty-state">
                    <i class="bi bi-list-ul"></i>
                    <p class="mb-0">Kullanıcının menü yetkisi bulunmuyor</p>
                </div>
            `;
        } else {
            menus.forEach(function(menu) {
                var sourceBadge = menu.source === 'Role' ? 
                    '<span class="badge bg-primary me-2">Rol</span>' : 
                    '<span class="badge bg-success me-2">Direkt</span>';
                
                var removeButton = menu.source === 'Direct' ? 
                    `<button class="btn btn-sm btn-outline-danger remove-user-menu" data-menu-id="${menu.id}" title="Yetkiyi Kaldır">
                        <i class="bi bi-x"></i>
                    </button>` : 
                    `<span class="text-muted small">Rol bazlı yetki</span>`;
                
                html += `
                    <div class="role-item d-flex justify-content-between align-items-center p-3 bg-light border rounded mb-2">
                        <div class="role-info">
                            <div class="d-flex align-items-center mb-1">
                                <i class="${menu.icon || 'bi bi-circle'} text-primary me-2"></i>
                                <span class="fw-bold">${menu.name}</span>
                                ${sourceBadge}
                            </div>
                            <div class="role-description">${menu.controller}/${menu.action || ''}</div>
                        </div>
                        <div class="role-actions">
                            ${removeButton}
                        </div>
                    </div>
                `;
            });
        }
        $('#assignedUserMenusList').html(html);
    }

    function displayAvailableUserMenus(menus) {
        var html = '';
        if (menus.length === 0) {
            html = '<div class="text-muted text-center py-3">Eklenecek menü bulunamadı</div>';
        } else {
            menus.forEach(function(menu) {
                html += `
                    <div class="d-flex justify-content-between align-items-center p-2 border-bottom">
                        <div>
                            <div class="fw-bold">
                                <i class="${menu.icon || 'bi bi-circle'} me-2"></i>
                                ${menu.name}
                            </div>
                            <small class="text-muted">${menu.controller}/${menu.action || ''}</small>
                        </div>
                        <button class="btn btn-sm btn-outline-success add-user-menu" data-menu-id="${menu.id}">
                            <i class="bi bi-plus"></i>
                        </button>
                    </div>
                `;
            });
        }
        $('#availableUserMenusList').html(html);
    }

    // Role Actions
    $(document).on('click', '.add-user-role', function() {
        var roleId = $(this).data('role-id');
        var userId = $('#userRoleModal').data('user-id');
        var $button = $(this);
        
        // Show loading state
        $button.html('<span class="loading-spinner"></span>').prop('disabled', true);
        
        $.ajax({
            url: '/User/AssignRoleToUser',
            type: 'POST',
            data: { userId: userId, roleId: roleId },
            success: function(response) {
                if (response.success) {
                    showRoleModalAlert(response.message, 'success');
                    loadUserRoles(userId);
                    loadAvailableRolesForUser(userId);
                } else {
                    showRoleModalAlert(response.error, 'error');
                }
            },
            error: function() {
                showRoleModalAlert('Rol atanırken hata oluştu!', 'error');
            },
            complete: function() {
                // Reset button state
                $button.html('<i class="bi bi-plus"></i>').prop('disabled', false);
            }
        });
    });

    $(document).on('click', '.remove-user-role', function() {
        var roleId = $(this).data('role-id');
        var userId = $('#userRoleModal').data('user-id');
        var $button = $(this);
        
        // Show loading state
        $button.html('<span class="loading-spinner"></span>').prop('disabled', true);
        
        $.ajax({
            url: '/User/RemoveRoleFromUser',
            type: 'DELETE',
            data: { userId: userId, roleId: roleId },
            success: function(response) {
                if (response.success) {
                    showRoleModalAlert(response.message, 'success');
                    loadUserRoles(userId);
                    loadAvailableRolesForUser(userId);
                } else {
                    showRoleModalAlert(response.error, 'error');
                }
            },
            error: function() {
                showRoleModalAlert('Rol kaldırılırken hata oluştu!', 'error');
            },
            complete: function() {
                // Reset button state
                $button.html('<i class="bi bi-x"></i>').prop('disabled', false);
            }
        });
    });

    // Menu Actions
    $(document).on('click', '.add-user-menu', function() {
        var menuId = $(this).data('menu-id');
        var userId = $('#userMenuPermissionsModal').data('user-id');
        var $button = $(this);
        
        // Show loading state
        $button.html('<span class="loading-spinner"></span>').prop('disabled', true);
        
        $.ajax({
            url: '/User/AssignMenuToUser',
            type: 'POST',
            data: { userId: userId, menuId: menuId },
            success: function(response) {
                if (response.success) {
                    showMenuModalAlert(response.message, 'success');
                    loadUserMenus(userId);
                    loadAvailableMenusForUser(userId);
                } else {
                    showMenuModalAlert(response.error, 'error');
                }
            },
            error: function() {
                showMenuModalAlert('Menü yetkisi verilirken hata oluştu!', 'error');
            },
            complete: function() {
                // Reset button state
                $button.html('<i class="bi bi-plus"></i>').prop('disabled', false);
            }
        });
    });

    $(document).on('click', '.remove-user-menu', function() {
        var menuId = $(this).data('menu-id');
        var userId = $('#userMenuPermissionsModal').data('user-id');
        var $button = $(this);
        
        // Show loading state
        $button.html('<span class="loading-spinner"></span>').prop('disabled', true);
        
        $.ajax({
            url: '/User/RemoveMenuFromUser',
            type: 'DELETE',
            data: { userId: userId, menuId: menuId },
            success: function(response) {
                if (response.success) {
                    showMenuModalAlert(response.message, 'success');
                    loadUserMenus(userId);
                    loadAvailableMenusForUser(userId);
                } else {
                    showMenuModalAlert(response.error, 'error');
                }
            },
            error: function() {
                showMenuModalAlert('Menü yetkisi kaldırılırken hata oluştu!', 'error');
            },
            complete: function() {
                // Reset button state
                $button.html('<i class="bi bi-x"></i>').prop('disabled', false);
            }
        });
    });

    // Search Functions
    $('#roleSearchInput').on('input', function() {
        var search = $(this).val();
        var userId = $('#userRoleModal').data('user-id');
        loadAvailableRolesForUser(userId, search);
    });

    $('#menuSearchInput').on('input', function() {
        var search = $(this).val();
        var userId = $('#userMenuPermissionsModal').data('user-id');
        loadAvailableMenusForUser(userId, search);
    });

    // Initialize
    initDataTable();
}
