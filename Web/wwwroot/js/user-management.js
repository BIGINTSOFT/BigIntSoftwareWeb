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
                { data: 'Id' },
                { data: 'Username' },
                { 
                    data: null,
                    render: function(data, type, row) {
                        return row.FirstName + ' ' + row.LastName;
                    }
                },
                { data: 'Email' },
                { 
                    data: 'IsActive',
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
                        return '<button class="btn btn-sm btn-outline-primary" onclick="openUserRoleModal(' + row.Id + ', \'' + row.Username + '\', \'' + row.FirstName + ' ' + row.LastName + '\', \'' + row.Email + '\', ' + row.IsActive + ')">' +
                               '<i class="bi bi-shield-check me-1"></i>Roller</button>';
                    }
                },
                {
                    data: null,
                    orderable: false,
                    render: function(data, type, row) {
                        return '<button class="btn btn-sm btn-outline-warning" onclick="openUserExtraPermissionsModal(' + row.Id + ', \'' + row.Username + '\', \'' + row.FirstName + ' ' + row.LastName + '\', \'' + row.Email + '\', ' + row.IsActive + ')">' +
                               '<i class="bi bi-gear me-1"></i>Özel Yetkiler</button>';
                    }
                },
                { data: 'CreatedDate' },
                { data: 'LastLoginDate' },
                {
                    data: null,
                    render: function(data, type, row) {
                        var actions = '';
                        
                        if (window.userPermissions && window.userPermissions.canView) {
                            actions += '<button class="btn btn-sm btn-outline-info me-1 view-user" data-id="' + row.Id + '" title="Görüntüle">';
                            actions += '<i class="bi bi-eye"></i></button>';
                        }
                        
                        if (window.userPermissions && window.userPermissions.canEdit) {
                            actions += '<button class="btn btn-sm btn-outline-primary me-1 edit-user" data-id="' + row.Id + '" title="Düzenle">';
                            actions += '<i class="bi bi-pencil"></i></button>';
                        }
                        
                        if (window.userPermissions && window.userPermissions.canDelete) {
                            actions += '<button class="btn btn-sm btn-outline-danger me-1 delete-user" data-id="' + row.Id + '" title="Sil">';
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
                    $('#userId').val(response.data.Id);
                    $('#username').val(response.data.Username);
                    $('#firstName').val(response.data.FirstName);
                    $('#lastName').val(response.data.LastName);
                    $('#email').val(response.data.Email);
                    $('#isActive').prop('checked', response.data.IsActive);
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
        $('#modalUserName').text(user.Username || '-');
        // firstName ve lastName'i birleştir ve trim yap
        var fullName = ((user.FirstName || '') + ' ' + (user.LastName || '')).trim();
        $('#modalUserFullName').text(fullName || '-');
        $('#modalUserEmail').text(user.Email || '-');
        
        const statusBadge = user.IsActive ? 
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
                                <span class="fw-bold">${role.Name}</span>
                                <span class="role-badge bg-primary text-white ms-2">Aktif</span>
                            </div>
                            <div class="role-description">${role.Description || 'Açıklama bulunmuyor'}</div>
                        </div>
                        <div class="role-actions">
                            <button class="btn btn-sm btn-outline-danger remove-user-role" data-role-id="${role.Id}" title="Rolü Kaldır">
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
                                <span class="fw-bold">${role.Name}</span>
                            </div>
                            <div class="role-description">${role.Description || 'Açıklama bulunmuyor'}</div>
                        </div>
                        <div class="role-actions">
                            <button class="btn btn-sm btn-outline-success add-user-role" data-role-id="${role.Id}" title="Role Ekle">
                                <i class="bi bi-plus"></i>
                            </button>
                        </div>
                    </div>
                `;
            });
        }
        $('#availableUserRolesList').html(html);
    }

    // User Extra Permissions Management (Yeni ERP Yapısı)
    $(document).on('click', '.view-extra-permissions', function() {
        var userId = $(this).data('id');
        openUserExtraPermissionsModal(userId);
    });

    function openUserExtraPermissionsModal(userId) {
        $('#userExtraPermissionsModal').modal('show');
        
        // Kullanıcı bilgilerini yükle
        loadUserInfo(userId);
        
        // Özel yetkileri yükle
        loadUserExtraPermissions(userId);
        loadAvailableMenusForExtraPermission(userId);
        
        // Store current user ID
        $('#userExtraPermissionsModal').data('user-id', userId);
    }

    function displayExtraPermissionUserInfo(user) {
        $('#modalExtraPermissionUserName').text(user.Username || '-');
        // firstName ve lastName'i birleştir
        var fullName = ((user.FirstName || '') + ' ' + (user.LastName || '')).trim();
        $('#modalExtraPermissionUserFullName').text(fullName || '-');
        $('#modalExtraPermissionUserEmail').text(user.Email || '-');
        
        var statusBadge = user.IsActive ? 
            '<span class="badge bg-success">Aktif</span>' : 
            '<span class="badge bg-danger">Pasif</span>';
        $('#modalExtraPermissionUserStatus').html(statusBadge);
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

    function loadUserExtraPermissions(userId) {
        $.ajax({
            url: '/User/GetUserExtraPermissions',
            type: 'GET',
            data: { userId: userId },
            success: function(response) {
                if (response.success) {
                    displayAssignedUserExtraPermissions(response.data);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Özel yetkiler yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function loadAvailableMenusForExtraPermission(userId, search = '') {
        $.ajax({
            url: '/User/GetAvailableMenusForExtraPermission',
            type: 'GET',
            data: { userId: userId, search: search },
            success: function(response) {
                if (response.success) {
                    displayAvailableMenusForExtraPermission(response.data);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Menüler yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function displayAssignedUserExtraPermissions(permissions) {
        var html = '';
        if (permissions.length === 0) {
            html = `
                <div class="empty-state">
                    <i class="bi bi-gear"></i>
                    <p class="mb-0">Kullanıcının özel yetkisi bulunmuyor</p>
                </div>
            `;
        } else {
        permissions.forEach(function(permission) {
            var permissionBadge = getPermissionBadgeClass(permission.PermissionLevel);
            
            html += `
                <div class="role-item d-flex justify-content-between align-items-center p-3 bg-light border rounded mb-2">
                    <div class="role-info">
                        <div class="d-flex align-items-center mb-1">
                            <i class="${permission.MenuIcon || 'bi bi-circle'} text-primary me-2"></i>
                            <span class="fw-bold">${permission.MenuName}</span>
                            <span class="badge ${permissionBadge} ms-2">${permission.PermissionLevel}</span>
                        </div>
                        <div class="role-description">${permission.MenuController}/${permission.MenuAction || ''}</div>
                        <div class="role-description text-muted small">Sebep: ${permission.Reason}</div>
                    </div>
                    <div class="role-actions">
                        <button class="btn btn-sm btn-outline-danger remove-user-extra-permission" data-permission-id="${permission.Id}" title="Yetkiyi Kaldır">
                            <i class="bi bi-x"></i>
                        </button>
                    </div>
                </div>
            `;
        });
        }
        $('#assignedUserExtraPermissionsList').html(html);
    }

    function displayAvailableMenusForExtraPermission(menus) {
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
                        <button class="btn btn-sm btn-outline-success add-user-extra-permission" data-menu-id="${menu.Id}" data-permission-level="VIEW">
                            <i class="bi bi-eye"></i> VIEW
                        </button>
                        <button class="btn btn-sm btn-outline-primary add-user-extra-permission" data-menu-id="${menu.Id}" data-permission-level="CREATE">
                            <i class="bi bi-plus"></i> CREATE
                        </button>
                        <button class="btn btn-sm btn-outline-warning add-user-extra-permission" data-menu-id="${menu.Id}" data-permission-level="EDIT">
                            <i class="bi bi-pencil"></i> EDIT
                        </button>
                        <button class="btn btn-sm btn-outline-danger add-user-extra-permission" data-menu-id="${menu.Id}" data-permission-level="DELETE">
                            <i class="bi bi-trash"></i> DELETE
                        </button>
                    </div>
                </div>
            `;
        });
        }
        $('#availableMenusForExtraPermissionList').html(html);
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

    // Extra Permission Actions (Yeni ERP Yapısı)
    $(document).on('click', '.add-user-extra-permission', function() {
        var menuId = $(this).data('menu-id');
        var permissionLevel = $(this).data('permission-level');
        var userId = $('#userExtraPermissionsModal').data('user-id');
        var $button = $(this);
        
        // Sebep sor
        var reason = prompt('Bu özel yetkiyi verme sebebinizi yazın:');
        if (!reason || reason.trim() === '') {
            showAlert('Sebep belirtilmelidir!', 'error');
            return;
        }
        
        // Show loading state
        $button.html('<span class="loading-spinner"></span>').prop('disabled', true);
        
        $.ajax({
            url: '/User/AssignExtraPermissionToUser',
            type: 'POST',
            data: { 
                userId: userId, 
                menuId: menuId, 
                permissionLevel: permissionLevel,
                reason: reason.trim()
            },
            success: function(response) {
                if (response.success) {
                    showAlert(response.message, 'success');
                    loadUserExtraPermissions(userId);
                    loadAvailableMenusForExtraPermission(userId);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Özel yetki verilirken hata oluştu!', 'error');
            },
            complete: function() {
                // Reset button state
                $button.html('<i class="bi bi-plus"></i>').prop('disabled', false);
            }
        });
    });

    $(document).on('click', '.remove-user-extra-permission', function() {
        var permissionId = $(this).data('permission-id');
        var userId = $('#userExtraPermissionsModal').data('user-id');
        var $button = $(this);
        
        // Show loading state
        $button.html('<span class="loading-spinner"></span>').prop('disabled', true);
        
        $.ajax({
            url: '/User/RemoveExtraPermissionFromUser',
            type: 'DELETE',
            data: { userId: userId, permissionId: permissionId },
            success: function(response) {
                if (response.success) {
                    showAlert(response.message, 'success');
                    loadUserExtraPermissions(userId);
                    loadAvailableMenusForExtraPermission(userId);
                } else {
                    showAlert(response.error, 'error');
                }
            },
            error: function() {
                showAlert('Özel yetki kaldırılırken hata oluştu!', 'error');
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

    $('#extraPermissionSearchInput').on('input', function() {
        var search = $(this).val();
        var userId = $('#userExtraPermissionsModal').data('user-id');
        loadAvailableMenusForExtraPermission(userId, search);
    });

    // Initialize
    initDataTable();
}

// Yardımcı Fonksiyonlar
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

// Global Functions for User Management
function openUserRoleModal(userId, username, fullName, email, isActive) {
    $('#userRoleModal').data('user-id', userId);
    $('#modalUserName').text(username);
    $('#modalUserFullName').text(fullName);
    $('#modalUserEmail').text(email);
    $('#modalUserStatus').text(isActive ? 'Aktif' : 'Pasif').removeClass('bg-success bg-danger').addClass(isActive ? 'bg-success' : 'bg-danger');
    
    loadUserRoles(userId);
    loadAvailableRolesForUser(userId);
    $('#userRoleModal').modal('show');
}

function openUserExtraPermissionsModal(userId, username, fullName, email, isActive) {
    $('#userExtraPermissionsModal').data('user-id', userId);
    $('#modalExtraPermissionUserName').text(username);
    $('#modalExtraPermissionUserFullName').text(fullName);
    $('#modalExtraPermissionUserEmail').text(email);
    $('#modalExtraPermissionUserStatus').text(isActive ? 'Aktif' : 'Pasif').removeClass('bg-success bg-danger').addClass(isActive ? 'bg-success' : 'bg-danger');
    
    loadUserExtraPermissions(userId);
    loadAvailableMenusForExtraPermission(userId);
    $('#userExtraPermissionsModal').modal('show');
}

function openUserSystemPermissionsModal(userId, username, fullName, email, isActive) {
    $('#userSystemPermissionsModal').data('user-id', userId);
    $('#modalPermissionUserName').text(username);
    $('#modalPermissionUserFullName').text(fullName);
    $('#modalPermissionUserEmail').text(email);
    $('#modalPermissionUserStatus').text(isActive ? 'Aktif' : 'Pasif').removeClass('bg-success bg-danger').addClass(isActive ? 'bg-success' : 'bg-danger');
    
    loadUserPermissions(userId);
    loadAvailablePermissionsForUser(userId);
    $('#userSystemPermissionsModal').modal('show');
}

// Load User Permissions
function loadUserPermissions(userId) {
    $.ajax({
        url: '/User/GetUserPermissions',
        type: 'GET',
        data: { userId: userId },
        success: function(response) {
            if (response.success) {
                var html = '';
                if (response.data.length > 0) {
                    response.data.forEach(function(permission) {
                        html += '<div class="d-flex justify-content-between align-items-center mb-2 p-2 border rounded">';
                        html += '<div>';
                        html += '<h6 class="mb-1">' + permission.name + '</h6>';
                        html += '<small class="text-muted">' + permission.code + '</small>';
                        if (permission.description) {
                            html += '<br><small class="text-muted">' + permission.description + '</small>';
                        }
                        html += '</div>';
                        html += '<button class="btn btn-sm btn-outline-danger remove-user-permission" data-permission-id="' + permission.id + '">';
                        html += '<i class="bi bi-x"></i>';
                        html += '</button>';
                        html += '</div>';
                    });
                } else {
                    html = '<div class="text-center text-muted py-4">';
                    html += '<i class="bi bi-gear fs-1"></i>';
                    html += '<p class="mt-2">Henüz sistem yetkisi atanmamış</p>';
                    html += '</div>';
                }
                $('#assignedUserPermissionsList').html(html);
            }
        },
        error: function() {
            $('#assignedUserPermissionsList').html('<div class="text-center text-danger py-4">Yetkiler yüklenirken hata oluştu!</div>');
        }
    });
}

// Load Available Permissions for User
function loadAvailablePermissionsForUser(userId, search = '') {
    $.ajax({
        url: '/User/GetAvailablePermissionsForUser',
        type: 'GET',
        data: { userId: userId, search: search },
        success: function(response) {
            if (response.success) {
                var html = '';
                if (response.data.length > 0) {
                    response.data.forEach(function(permission) {
                        html += '<div class="d-flex justify-content-between align-items-center mb-2 p-2 border rounded">';
                        html += '<div>';
                        html += '<h6 class="mb-1">' + permission.name + '</h6>';
                        html += '<small class="text-muted">' + permission.code + '</small>';
                        if (permission.description) {
                            html += '<br><small class="text-muted">' + permission.description + '</small>';
                        }
                        html += '</div>';
                        html += '<button class="btn btn-sm btn-outline-success add-user-permission" data-permission-id="' + permission.id + '">';
                        html += '<i class="bi bi-plus"></i>';
                        html += '</button>';
                        html += '</div>';
                    });
                } else {
                    html = '<div class="text-center text-muted py-4">';
                    html += '<i class="bi bi-search fs-1"></i>';
                    html += '<p class="mt-2">Arama kriterlerine uygun yetki bulunamadı</p>';
                    html += '</div>';
                }
                $('#availableUserPermissionsList').html(html);
            }
        },
        error: function() {
            $('#availableUserPermissionsList').html('<div class="text-center text-danger py-4">Yetkiler yüklenirken hata oluştu!</div>');
        }
    });
}

// Permission Actions
$(document).on('click', '.add-user-permission', function() {
    var permissionId = $(this).data('permission-id');
    var userId = $('#userSystemPermissionsModal').data('user-id');
    var $button = $(this);
    
    // Show loading state
    $button.html('<span class="loading-spinner"></span>').prop('disabled', true);
    
    $.ajax({
        url: '/User/AssignPermissionToUser',
        type: 'POST',
        data: { userId: userId, permissionId: permissionId },
        success: function(response) {
            if (response.success) {
                showPermissionModalAlert(response.message, 'success');
                loadUserPermissions(userId);
                loadAvailablePermissionsForUser(userId);
            } else {
                showPermissionModalAlert(response.error, 'error');
            }
        },
        error: function() {
            showPermissionModalAlert('Sistem yetkisi verilirken hata oluştu!', 'error');
        },
        complete: function() {
            // Reset button state
            $button.html('<i class="bi bi-plus"></i>').prop('disabled', false);
        }
    });
});

$(document).on('click', '.remove-user-permission', function() {
    var permissionId = $(this).data('permission-id');
    var userId = $('#userSystemPermissionsModal').data('user-id');
    var $button = $(this);
    
    // Show loading state
    $button.html('<span class="loading-spinner"></span>').prop('disabled', true);
    
    $.ajax({
        url: '/User/RemovePermissionFromUser',
        type: 'DELETE',
        data: { userId: userId, permissionId: permissionId },
        success: function(response) {
            if (response.success) {
                showPermissionModalAlert(response.message, 'success');
                loadUserPermissions(userId);
                loadAvailablePermissionsForUser(userId);
            } else {
                showPermissionModalAlert(response.error, 'error');
            }
        },
        error: function() {
            showPermissionModalAlert('Sistem yetkisi kaldırılırken hata oluştu!', 'error');
        },
        complete: function() {
            // Reset button state
            $button.html('<i class="bi bi-x"></i>').prop('disabled', false);
        }
    });
});

// Show Permission Modal Alert
function showPermissionModalAlert(message, type) {
    var alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
    var alertHtml = '<div class="alert ' + alertClass + ' alert-dismissible fade show" role="alert">' +
                   '<span>' + message + '</span>' +
                   '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
                   '</div>';
    
    $('#permissionModalAlert').html(alertHtml).show();
    
    // Auto hide after 3 seconds
    setTimeout(function() {
        $('#permissionModalAlert').fadeOut();
    }, 3000);
}
