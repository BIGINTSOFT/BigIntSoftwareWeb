// User Management JavaScript with DevExtreme - Complete Implementation
$(document).ready(function() {
    initializeUserManagement();
});

function initializeUserManagement() {
    let usersDataGrid;
    let currentUserId = null;
    let isEditMode = false;

    // Initialize DevExtreme DataGrid
    function initDataGrid() {
        usersDataGrid = $("#usersDataGrid").dxDataGrid({
            dataSource: {
                load: function(loadOptions) {
                    return $.ajax({
                url: '/User/GetUsers',
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
                fileName: 'KullaniciListesi',
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
                    dataField: 'Username',
                    caption: 'Kullanıcı Adı',
                    width: 120
                },
                {
                    caption: 'Ad Soyad',
                    width: 140,
                    allowFiltering: true,
                    allowHeaderFiltering: true,
                    cellTemplate: function(container, options) {
                        const user = options.data;
                        const fullName = `${user.FirstName || ''} ${user.LastName || ''}`.trim();
                        container.text(fullName || '-');
                    }
                },
                {
                    dataField: 'Email',
                    caption: 'Email',
                    width: 180
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
                    caption: 'Roller',
                    width: 80,
                    alignment: 'center',
                    allowSorting: false,
                    allowFiltering: false,
                    allowHeaderFiltering: false,
                    cellTemplate: function(container, options) {
                        const user = options.data;
                        const button = $('<button>')
                            .addClass('dx-action-btn dx-action-btn-roles')
                            .attr('title', 'Rol Yönetimi')
                            .html('<i class="bi bi-shield-check"></i>')
                            .on('click', function() {
                                openUserRoleModal(user.Id, user.Username, `${user.FirstName} ${user.LastName}`, user.Email, user.IsActive);
                            });
                        container.append(button);
                    }
                },
                {
                    caption: 'Yetkiler',
                    width: 80,
                    alignment: 'center',
                    allowSorting: false,
                    allowFiltering: false,
                    allowHeaderFiltering: false,
                    cellTemplate: function(container, options) {
                        const user = options.data;
                        const button = $('<button>')
                            .addClass('dx-action-btn dx-action-btn-permissions')
                            .attr('title', 'Menü ve Yetki Yönetimi')
                            .html('<i class="bi bi-gear"></i>')
                            .on('click', function() {
                                openUserPermissionsModal(user.Id, user.Username, `${user.FirstName} ${user.LastName}`, user.Email, user.IsActive);
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
                        const user = options.data;
                        const actionsContainer = $('<div>').addClass('dx-action-buttons');
                        
                        if (window.userPermissions && window.userPermissions.canView) {
                            const viewBtn = $('<button>')
                                .addClass('dx-action-btn dx-action-btn-view')
                                .attr('title', 'Görüntüle')
                                .html('<i class="bi bi-eye"></i>')
                                .on('click', function() {
                                    viewUser(user.Id);
                                });
                            actionsContainer.append(viewBtn);
                        }
                        
                        if (window.userPermissions && window.userPermissions.canEdit) {
                            const editBtn = $('<button>')
                                .addClass('dx-action-btn dx-action-btn-edit')
                                .attr('title', 'Düzenle')
                                .html('<i class="bi bi-pencil"></i>')
                                .on('click', function() {
                                    editUser(user.Id);
                                });
                            actionsContainer.append(editBtn);
                        }
                        
                        if (window.userPermissions && window.userPermissions.canDelete) {
                            const deleteBtn = $('<button>')
                                .addClass('dx-action-btn dx-action-btn-delete')
                                .attr('title', 'Sil')
                                .html('<i class="bi bi-trash"></i>')
                                .on('click', function() {
                                    deleteUser(user.Id);
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

    // Add User Button
    $('#addUserBtn').click(function() {
        if (!window.userPermissions || !window.userPermissions.canCreate) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        isEditMode = false;
        currentUserId = null;
        $('#userModalLabel').text('Yeni Kullanıcı');
        $('#userForm')[0].reset();
        $('#userId').val('');
        $('#passwordHelp').show();
        $('#password').attr('required', true);
        $('#generatePasswordBtn').show();
        $('#resetPasswordBtn').hide();
        showSlideModal();
    });

    // Refresh Button
    $('#refreshBtn').click(function() {
        usersDataGrid.refresh();
        showAlert('Tablo yenilendi', 'success');
    });

    // Export Button - Fixed exportToExcel issue
    $('#exportBtn').click(function() {
        try {
            if (usersDataGrid && typeof usersDataGrid.exportToExcel === 'function') {
        usersDataGrid.exportToExcel({
            fileName: 'KullaniciListesi_' + new Date().toISOString().split('T')[0],
            autoFilterEnabled: true
        });
            } else {
                // Fallback export method
                exportToExcelFallback();
            }
        } catch (error) {
            console.error('Export error:', error);
            exportToExcelFallback();
        }
    });

    // Fallback export method
    function exportToExcelFallback() {
        showAlert('Excel export özelliği şu anda kullanılamıyor. Verileri manuel olarak kopyalayabilirsiniz.', 'warning');
    }

    // User Action Functions
    function viewUser(userId) {
        if (!window.userPermissions || !window.userPermissions.canView) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        $.get('/User/GetUser/' + userId)
            .done(function(response) {
                if (response.data) {
                    const user = response.data;
                    showUserDetails(user);
                } else {
                    showAlert('Kullanıcı bilgileri alınamadı', 'error');
                }
            })
            .fail(function() {
                showAlert('Kullanıcı bilgileri alınamadı', 'error');
            });
    }

    function editUser(userId) {
        if (!window.userPermissions || !window.userPermissions.canEdit) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
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
    }

    function deleteUser(userId) {
        if (!window.userPermissions || !window.userPermissions.canDelete) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        currentUserId = userId;
        $('#deleteModal').modal('show');
    }

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

            const url = isEditMode ? '/User/UpdateUser' : '/User/CreateUser';
            const method = 'POST';

            $.ajax({
                url: url,
                type: method,
                contentType: 'application/json',
                data: JSON.stringify(formData),
                success: function(response) {
                    if (response.success) {
                        hideSlideModal();
                        usersDataGrid.refresh();
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
            url: '/User/DeleteUser',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ id: currentUserId }),
            success: function(response) {
                if (response.success) {
                    $('#deleteModal').modal('hide');
                    usersDataGrid.refresh();
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

    // User Role Management Functions
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

    function loadUserRoles(userId) {
        $.ajax({
            url: '/User/GetUserRoles',
            type: 'GET',
            data: { userId: userId },
            success: function(response) {
                if (response.success) {
                    displayAssignedUserRoles(response.data);
                } else {
                    showRoleModalAlert(response.message || response.error, 'error');
                }
            },
            error: function() {
                showRoleModalAlert('Roller yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function loadAvailableRolesForUser(userId, search = '') {
        // Use RoleController endpoint to get all roles, then filter out assigned ones
        $.ajax({
            url: '/Role/GetRoles',
            type: 'GET',
            success: function(response) {
                if (response.success) {
                    // Get assigned roles to filter them out
                    $.ajax({
                        url: '/User/GetUserRoles',
                        type: 'GET',
                        data: { userId: userId },
                        success: function(userRolesResponse) {
                            if (userRolesResponse.success) {
                                const assignedRoleIds = userRolesResponse.data.map(role => role.Id);
                                const availableRoles = response.data.filter(role => !assignedRoleIds.includes(role.Id));
                                
                                // Apply search filter
                                if (search) {
                                    availableRoles = availableRoles.filter(role => 
                                        role.Name.toLowerCase().includes(search.toLowerCase()) ||
                                        (role.Description && role.Description.toLowerCase().includes(search.toLowerCase()))
                                    );
                                }
                                
                                displayAvailableUserRoles(availableRoles);
                            } else {
                    displayAvailableUserRoles(response.data);
                            }
                        },
                        error: function() {
                            displayAvailableUserRoles(response.data);
                        }
                    });
                } else {
                    showRoleModalAlert(response.message || response.error, 'error');
                }
            },
            error: function() {
                showRoleModalAlert('Roller yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function displayAssignedUserRoles(roles) {
        var html = '';
        if (roles.length === 0) {
            html = `
                <div class="empty-state text-center py-4">
                    <i class="bi bi-shield-x fs-1 text-muted"></i>
                    <p class="mb-0 text-muted small">Kullanıcının atanmış rolü bulunmuyor</p>
                </div>
            `;
        } else {
            roles.forEach(function(role) {
                const roleName = role.Name || role.name || role.roleName || role.RoleName || '-';
                const roleDesc = role.Description || role.description || role.RoleDescription || '';
                const roleId = role.Id || role.id || role.RoleId;
                html += `
                    <div class="role-item d-flex justify-content-between align-items-center p-2 bg-light border rounded mb-1">
                        <div class="role-info">
                            <div class="d-flex align-items-center">
                                <i class="bi bi-shield-check text-primary me-2 small"></i>
                                <span class="fw-bold small">${roleName}</span>
                                <span class="badge bg-success ms-2 small">Aktif</span>
                            </div>
                            <div class="role-description small text-muted">${roleDesc || 'Açıklama bulunmuyor'}</div>
                        </div>
                        <div class="role-actions">
                            <button class="btn btn-sm btn-outline-danger remove-user-role" data-role-id="${roleId}" title="Rolü Kaldır">
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
                <div class="empty-state text-center py-4">
                    <i class="bi bi-shield-plus fs-1 text-muted"></i>
                    <p class="mb-0 text-muted small">Eklenecek rol bulunamadı</p>
                </div>
            `;
        } else {
            roles.forEach(function(role) {
                const roleName = role.Name || role.name || role.roleName || role.RoleName || '-';
                const roleDesc = role.Description || role.description || role.RoleDescription || '';
                const roleId = role.Id || role.id || role.RoleId;
                html += `
                    <div class="role-item d-flex justify-content-between align-items-center p-2 bg-white border rounded mb-1">
                        <div class="role-info">
                            <div class="d-flex align-items-center">
                                <i class="bi bi-shield text-success me-2 small"></i>
                                <span class="fw-bold small">${roleName}</span>
                            </div>
                            <div class="role-description small text-muted">${roleDesc || 'Açıklama bulunmuyor'}</div>
                        </div>
                        <div class="role-actions">
                            <button class="btn btn-sm btn-success add-user-role" data-role-id="${roleId}" title="Role Ekle">
                                <i class="bi bi-plus"></i>
                            </button>
                        </div>
                    </div>
                `;
            });
        }
        $('#availableUserRolesList').html(html);
    }

    // User Permissions Management Functions (Menu + Permission combined)
    function openUserPermissionsModal(userId, username, fullName, email, isActive) {
        $('#userPermissionsModal').data('user-id', userId);
        $('#modalPermissionUserName').text(username);
        $('#modalPermissionUserFullName').text(fullName);
        $('#modalPermissionUserEmail').text(email);
        $('#modalPermissionUserStatus').text(isActive ? 'Aktif' : 'Pasif').removeClass('bg-success bg-danger').addClass(isActive ? 'bg-success' : 'bg-danger');
        
        loadUserMenuPermissions(userId);
        loadAvailableMenusForUser(userId);
        $('#userPermissionsModal').modal('show');
    }

    function loadUserMenuPermissions(userId) {
        $.ajax({
            url: '/User/GetUserMenuPermissions',
            type: 'GET',
            data: { userId: userId },
            success: function(response) {
                if (response.success) {
                    displayAssignedUserMenuPermissions(response.data);
                } else {
                    showPermissionModalAlert(response.message || response.error, 'error');
                }
            },
            error: function() {
                showPermissionModalAlert('Menü yetkileri yüklenirken hata oluştu!', 'error');
            }
        });
    }

  

    function loadAvailableMenusForUser(userId, search = '') {
        // Yeni endpoint kullan - kullanıcının rollerindeki yetkileri de filtreler
        $.ajax({
            url: '/User/GetAvailableMenusAndPermissionsForUser',
            type: 'GET',
            data: { userId: userId },
            success: function(response) {
                if (response.success) {
                    let availableMenus = response.data;
                    
                    // Apply search filter
                    if (search) {
                        availableMenus = availableMenus.filter(menu => 
                            menu.menuName.toLowerCase().includes(search.toLowerCase()) ||
                            (menu.menuController && menu.menuController.toLowerCase().includes(search.toLowerCase())) ||
                            (menu.menuAction && menu.menuAction.toLowerCase().includes(search.toLowerCase()))
                        );
                    }
                    
                    displayAvailableMenusForUser(availableMenus);
                } else {
                    showPermissionModalAlert(response.message || response.error, 'error');
                }
            },
            error: function(xhr) {
                console.error('Error loading available menus:', xhr);
                showPermissionModalAlert('Menüler yüklenirken hata oluştu!', 'error');
            }
        });
    }

    function displayAssignedUserMenuPermissions(permissions) {
        console.log('Permissions received:', permissions);
        var html = '';
        
        if (permissions.length === 0) {
            html = `
                <div class="empty-state text-center py-4">
                    <i class="bi bi-gear fs-1 text-muted"></i>
                    <p class="mb-0 text-muted small">Kullanıcının menü yetkisi bulunmuyor</p>
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
                                    <button class="btn btn-sm btn-outline-danger remove-all-menu-permissions" 
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
        $('#assignedUserPermissionsList').html(html);
    }

    function displayAvailableMenusForUser(menus) {
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
                                ${menu.Name || menu.name}
                        </div>
                            <small class="text-muted" style="font-size: 11px;">${menu.Controller || menu.controller}/${menu.Action || menu.action || ''}</small>
                    </div>
                        <div class="ms-2">
                            <button class="btn btn-sm btn-success add-user-menu-permission" 
                                    data-menu-id="${menu.Id || menu.id}" 
                                    data-menu-name="${menu.Name || menu.name}"
                                    title="Menü ve Yetki Ekle">
                                <i class="bi bi-plus-circle me-1"></i>
                                Ekle
                        </button>
                    </div>
                </div>
            `;
        });
        }
        $('#availableUserPermissionsList').html(html);
    }

    // Role Actions
    $(document).on('click', '.add-user-role', function() {
        var roleId = $(this).data('role-id');
        var userId = $('#userRoleModal').data('user-id');
        var $button = $(this);
        
        // Show loading state
        $button.html('<span class="spinner-border spinner-border-sm"></span>').prop('disabled', true);
        
        $.ajax({
            url: '/User/AssignRoleToUser',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ userId: userId, roleId: roleId }),
            success: function(response) {
                if (response.success) {
                    showRoleModalAlert(response.message, 'success');
                    loadUserRoles(userId);
                    loadAvailableRolesForUser(userId);
                } else {
                    showRoleModalAlert(response.message || response.error, 'error');
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
        
        if (confirm('Bu rolü kaldırmak istediğinizden emin misiniz?')) {
        // Show loading state
            $button.html('<span class="spinner-border spinner-border-sm"></span>').prop('disabled', true);
        
        $.ajax({
            url: '/User/RemoveRoleFromUser',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ userId: userId, roleId: roleId }),
            success: function(response) {
                if (response.success) {
                    showRoleModalAlert(response.message, 'success');
                    loadUserRoles(userId);
                    loadAvailableRolesForUser(userId);
                } else {
                        showRoleModalAlert(response.message || response.error, 'error');
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
        }
    });

    // Menu Permission Actions
    $(document).on('click', '.add-user-menu-permission', function() {
        var menuId = $(this).data('menu-id');
        var menuName = $(this).data('menu-name');
        var userId = $('#userPermissionsModal').data('user-id');
        var availablePermissions = $(this).data('available-permissions');
        
        // Show menu and permission selection modal (sağdan sola açılır)
        showMenuPermissionSelectionModal(menuId, menuName, userId, availablePermissions);
    });

    // Remove all permissions for a specific menu
    $(document).on('click', '.remove-all-menu-permissions', function() {
        var menuId = $(this).data('menu-id');
        var userId = $('#userPermissionsModal').data('user-id');
        var $button = $(this);
        
        if (confirm('Bu menüye ait tüm yetkileri kaldırmak istediğinizden emin misiniz?')) {
        // Show loading state
            $button.html('<span class="spinner-border spinner-border-sm"></span>').prop('disabled', true);
        
        $.ajax({
                url: '/User/RemoveAllMenuPermissions',
            type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ 
                userId: userId, 
                    menuId: menuId
                }),
            success: function(response) {
                if (response.success) {
                        showPermissionModalAlert(response.message, 'success');
                        loadUserMenuPermissions(userId);
                        loadAvailableMenusForUser(userId);
                } else {
                        showPermissionModalAlert(response.message || response.error, 'error');
                }
            },
            error: function() {
                    showPermissionModalAlert('Menü yetkileri kaldırılırken hata oluştu!', 'error');
            },
            complete: function() {
                // Reset button state
                    $button.html('<i class="bi bi-trash"></i>').prop('disabled', false);
                }
            });
        }
    });

    // Menu and Permission Selection Modal - Dynamic Version with Role Filtering
    function showMenuPermissionSelectionModal(menuId, menuName, userId, availablePermissions) {
        // Direkt kullanılabilir permission'ları kullan (rollerden filtrelenmiş)
        if (availablePermissions && availablePermissions.length > 0) {
            buildAndShowPermissionModal(menuId, menuName, userId, availablePermissions);
        } else {
            showPermissionModalAlert('Bu menü için eklenebilir yetki bulunmamaktadır. Tüm yetkiler rollerinizden gelmektedir.', 'warning');
        }
    }
    
    // Build and show modal with dynamic permissions - SLIDE FROM RIGHT
    function buildAndShowPermissionModal(menuId, menuName, userId, permissions) {
        // Permission checkboxları dinamik olarak oluştur
        let permissionCheckboxes = '';
        const iconMap = {
            'VIEW': 'bi-eye text-primary',
            'CREATE': 'bi-plus text-success',
            'EDIT': 'bi-pencil text-warning',
            'UPDATE': 'bi-pencil text-warning',
            'DELETE': 'bi-trash text-danger'
        };
        
        permissions.forEach(function(permission, index) {
            const permName = permission.Name || permission.name || '';
            const permCode = permission.Code || permission.code || permName;
            const permDesc = permission.Description || permission.description || '';
            const permId = permission.Id || permission.id;
            const icon = iconMap[permCode.toUpperCase()] || 'bi-shield text-secondary';
            
            permissionCheckboxes += `
                <div class="form-check">
                    <input class="form-check-input permission-checkbox" type="checkbox" 
                           id="permission_${permId}" 
                           value="${permCode}" 
                           data-permission-id="${permId}">
                    <label class="form-check-label" for="permission_${permId}">
                        <i class="${icon} me-1"></i>
                        <strong>${permName}</strong>${permDesc ? ' - ' + permDesc : ''}
                    </label>
                </div>
            `;
        });
        
        const modalHtml = `
            <div class="modal fade" id="menuPermissionModal" tabindex="-1">
                <div class="modal-dialog modal-dialog-slide-right modal-lg">
                    <div class="modal-content">
                        <div class="modal-header bg-primary text-white">
                            <h5 class="modal-title">
                                <i class="bi bi-gear me-2"></i>
                                Menü ve Yetki Seçimi
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
                                    Sebep <span class="text-danger">*</span>
                                </label>
                                <textarea class="form-control" id="reasonInput" rows="3" 
                                          placeholder="Bu yetkileri verme sebebinizi yazın..."></textarea>
                            </div>
                            
                            <div class="mt-3">
                                <div class="alert alert-info">
                                    <i class="bi bi-info-circle me-2"></i>
                                    <strong>Bilgi:</strong> Seçtiğiniz yetkiler bu menü için kullanıcıya atanacaktır.
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                                <i class="bi bi-x-lg me-1"></i>
                                İptal
                            </button>
                            <button type="button" class="btn btn-primary" onclick="assignMultipleMenuPermissions(${menuId}, ${userId})">
                                <i class="bi bi-check-lg me-1"></i>
                                Yetkileri Ata
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `;
        
        // Remove existing modal if any
        $('#menuPermissionModal').remove();
        
        // Add new modal to body
        $('body').append(modalHtml);
        
        // Show modal with slide animation from right
        const modal = $('#menuPermissionModal');
        const modalDialog = modal.find('.modal-dialog-slide-right');
        
        // Add backdrop
        $('body').append('<div class="modal-backdrop fade show" id="permissionModalBackdrop"></div>');
        
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
        
        // Remove modal from DOM when hidden
        modal.on('hidden.bs.modal', function() {
            $('#permissionModalBackdrop').remove();
            $('body').removeClass('modal-open');
            $(this).remove();
        });
        
        // Close on backdrop click
        $(document).on('click', '#permissionModalBackdrop', function() {
            hidePermissionModal();
        });
        
        // Close button handler
        modal.find('.btn-close, [data-bs-dismiss="modal"]').on('click', function() {
            hidePermissionModal();
        });
    }
    
    // Hide permission modal with animation
    function hidePermissionModal() {
        const modal = $('#menuPermissionModal');
        const modalDialog = modal.find('.modal-dialog-slide-right');
        
        // Hide slide animation
        modalDialog.removeClass('show');
        
        // Wait for animation to complete
        setTimeout(function() {
            modal.removeClass('show');
            modal.attr('aria-hidden', 'true');
            modal.css('display', 'none');
            
            // Remove backdrop
            $('#permissionModalBackdrop').remove();
            
            // Restore body scroll
            $('body').removeClass('modal-open');
            
            // Trigger hidden event
            modal.trigger('hidden.bs.modal');
        }, 300);
    }

    // Global function for assigning multiple menu permissions - DYNAMIC VERSION
    window.assignMultipleMenuPermissions = function(menuId, userId) {
        const reason = $('#reasonInput').val().trim();
        
        if (!reason) {
            alert('Sebep belirtilmelidir!');
            return;
        }
        
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
        hidePermissionModal();
        
        // Show loading
        showPermissionModalAlert(`${selectedPermissions.length} yetki atanıyor...`, 'info');
        
        // Assign permissions one by one
        let completed = 0;
        let errors = [];
        
        selectedPermissions.forEach(function(permission, index) {
        $.ajax({
                url: '/User/AssignMenuPermissionToUser',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    userId: userId,
                    menuId: menuId,
                    permissionId: permission.id, // Gerçek permission ID'si
                    permissionLevel: permission.code, // Permission code (VIEW, CREATE, etc.)
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
                            showPermissionModalAlert(`${selectedPermissions.length} yetki başarıyla atandı!`, 'success');
                        } else {
                            showPermissionModalAlert(`${completed - errors.length} yetki atandı, ${errors.length} hata oluştu`, 'warning');
                        }
                        loadUserMenuPermissions(userId);
                        loadAvailableMenusForUser(userId);
                    }
                },
                error: function(xhr) {
                    completed++;
                    errors.push(`${permission.name}: Sunucu hatası`);
                    
                    if (completed === selectedPermissions.length) {
                        if (errors.length === selectedPermissions.length) {
                            showPermissionModalAlert('Tüm yetkiler atanırken hata oluştu!', 'error');
                        } else {
                            showPermissionModalAlert(`${completed - errors.length} yetki atandı, ${errors.length} hata oluştu`, 'warning');
                        }
                        loadUserMenuPermissions(userId);
                        loadAvailableMenusForUser(userId);
                    }
                }
            });
        });
    };


    // Search Functions
    $('#roleSearchInput').on('input', function() {
        var search = $(this).val();
        var userId = $('#userRoleModal').data('user-id');
        loadAvailableRolesForUser(userId, search);
    });

    $('#permissionSearchInput').on('input', function() {
        var search = $(this).val();
        var userId = $('#userPermissionsModal').data('user-id');
        loadAvailableMenusForUser(userId, search);
    });

    // User Details Modal
    function showUserDetails(user) {
        const modalHtml = `
            <div class="modal fade" id="userDetailsModal" tabindex="-1">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Kullanıcı Detayları</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <div class="row">
                                <div class="col-md-6">
                                    <h6>Kullanıcı Bilgileri</h6>
                                    <p><strong>ID:</strong> ${user.Id}</p>
                                    <p><strong>Kullanıcı Adı:</strong> ${user.Username}</p>
                                    <p><strong>Ad:</strong> ${user.FirstName || '-'}</p>
                                    <p><strong>Soyad:</strong> ${user.LastName || '-'}</p>
                                    <p><strong>Email:</strong> ${user.Email}</p>
                                    <p><strong>Durum:</strong> <span class="badge ${user.IsActive ? 'bg-success' : 'bg-danger'}">${user.IsActive ? 'Aktif' : 'Pasif'}</span></p>
                                </div>
                                <div class="col-md-6">
                                    <h6>Zaman Bilgileri</h6>
                                    <p><strong>Oluşturulma:</strong> ${user.CreatedDate ? new Date(user.CreatedDate).toLocaleString('tr-TR') : '-'}</p>
                                    <p><strong>Son Güncelleme:</strong> ${user.UpdatedDate ? new Date(user.UpdatedDate).toLocaleString('tr-TR') : '-'}</p>
                                    <p><strong>Son Giriş:</strong> ${user.LastLoginDate ? new Date(user.LastLoginDate).toLocaleString('tr-TR') : '-'}</p>
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
        $('#userDetailsModal').remove();
        
        // Add new modal to body
        $('body').append(modalHtml);
        
        // Show modal
        $('#userDetailsModal').modal('show');
        
        // Remove modal from DOM when hidden
        $('#userDetailsModal').on('hidden.bs.modal', function() {
            $(this).remove();
        });
    }

    // Modal Alert Functions
    function showRoleModalAlert(message, type) {
        var alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
        $('#roleModalAlert')
            .removeClass('alert-success alert-danger')
            .addClass(alertClass)
            .find('#roleModalAlertMessage')
            .text(message);
        $('#roleModalAlert').show();
        
        // 3 saniye sonra otomatik gizle
        setTimeout(function() {
            $('#roleModalAlert').fadeOut();
        }, 3000);
    }

    function showPermissionModalAlert(message, type) {
        var alertClass = type === 'success' ? 'alert-success' : (type === 'info' ? 'alert-info' : 'alert-danger');
        $('#permissionModalAlert')
            .removeClass('alert-success alert-danger alert-info')
            .addClass(alertClass)
            .find('#permissionModalAlertMessage')
            .text(message);
        $('#permissionModalAlert').show();
        
        // 3 saniye sonra otomatik gizle
        setTimeout(function() {
            $('#permissionModalAlert').fadeOut();
        }, 3000);
    }

    // Initialize
    initDataGrid();
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

function openUserPermissionsModal(userId, username, fullName, email, isActive) {
    $('#userPermissionsModal').data('user-id', userId);
    $('#modalPermissionUserName').text(username);
    $('#modalPermissionUserFullName').text(fullName);
    $('#modalPermissionUserEmail').text(email);
    $('#modalPermissionUserStatus').text(isActive ? 'Aktif' : 'Pasif').removeClass('bg-success bg-danger').addClass(isActive ? 'bg-success' : 'bg-danger');
    
    loadUserMenuPermissions(userId);
    loadAvailableMenusForUser(userId);
    $('#userPermissionsModal').modal('show');
}

// Global Functions for Role Management
function loadUserRoles(userId) {
    $.ajax({
        url: '/User/GetUserRoles',
        type: 'GET',
        data: { userId: userId },
        success: function(response) {
            if (response.success) {
                displayAssignedUserRoles(response.data);
            } else {
                showRoleModalAlert(response.message || response.error, 'error');
            }
        },
        error: function() {
            showRoleModalAlert('Roller yüklenirken hata oluştu!', 'error');
        }
    });
}

function loadAvailableRolesForUser(userId, search = '') {
    // Use RoleController endpoint to get all roles, then filter out assigned ones
    $.ajax({
        url: '/Role/GetRoles',
        type: 'GET',
        success: function(response) {
            if (response.success) {
                // Get assigned roles to filter them out
                $.ajax({
                    url: '/User/GetUserRoles',
                    type: 'GET',
                    data: { userId: userId },
                    success: function(userRolesResponse) {
                        if (userRolesResponse.success) {
                            const assignedRoleIds = userRolesResponse.data.map(role => role.Id);
                            const availableRoles = response.data.filter(role => !assignedRoleIds.includes(role.Id));
                            
                            // Apply search filter
                            if (search) {
                                availableRoles = availableRoles.filter(role => 
                                    role.Name.toLowerCase().includes(search.toLowerCase()) ||
                                    (role.Description && role.Description.toLowerCase().includes(search.toLowerCase()))
                                );
                            }
                            
                            displayAvailableUserRoles(availableRoles);
            } else {
                            displayAvailableUserRoles(response.data);
            }
        },
        error: function() {
                        displayAvailableUserRoles(response.data);
                    }
                });
            } else {
                showRoleModalAlert(response.message || response.error, 'error');
            }
        },
        error: function() {
            showRoleModalAlert('Roller yüklenirken hata oluştu!', 'error');
        }
    });
}

function displayAssignedUserRoles(roles) {
    var html = '';
    if (roles.length === 0) {
        html = `
            <div class="empty-state text-center py-4">
                <i class="bi bi-shield-x fs-1 text-muted"></i>
                <p class="mb-0 text-muted small">Kullanıcının atanmış rolü bulunmuyor</p>
            </div>
        `;
    } else {
        roles.forEach(function(role) {
            const roleName = role.Name || role.name || role.roleName || role.RoleName || '-';
            const roleDesc = role.Description || role.description || role.RoleDescription || '';
            const roleId = role.Id || role.id || role.RoleId;
            html += `
                <div class="role-item d-flex justify-content-between align-items-center p-2 bg-light border rounded mb-1">
                    <div class="role-info">
                        <div class="d-flex align-items-center">
                            <i class="bi bi-shield-check text-primary me-2 small"></i>
                            <span class="fw-bold small">${roleName}</span>
                            <span class="badge bg-success ms-2 small">Aktif</span>
                        </div>
                        <div class="role-description small text-muted">${roleDesc || 'Açıklama bulunmuyor'}</div>
                    </div>
                    <div class="role-actions">
                        <button class="btn btn-sm btn-outline-danger remove-user-role" data-role-id="${roleId}" title="Rolü Kaldır">
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
            <div class="empty-state text-center py-4">
                <i class="bi bi-shield-plus fs-1 text-muted"></i>
                <p class="mb-0 text-muted small">Eklenecek rol bulunamadı</p>
            </div>
        `;
    } else {
        roles.forEach(function(role) {
            const roleName = role.Name || role.name || role.roleName || role.RoleName || '-';
            const roleDesc = role.Description || role.description || role.RoleDescription || '';
            const roleId = role.Id || role.id || role.RoleId;
            html += `
                <div class="role-item d-flex justify-content-between align-items-center p-2 bg-white border rounded mb-1">
                    <div class="role-info">
                        <div class="d-flex align-items-center">
                            <i class="bi bi-shield text-success me-2 small"></i>
                            <span class="fw-bold small">${roleName}</span>
                        </div>
                        <div class="role-description small text-muted">${roleDesc || 'Açıklama bulunmuyor'}</div>
                    </div>
                    <div class="role-actions">
                        <button class="btn btn-sm btn-success add-user-role" data-role-id="${roleId}" title="Role Ekle">
                            <i class="bi bi-plus"></i>
                        </button>
                    </div>
                </div>
            `;
        });
    }
    $('#availableUserRolesList').html(html);
}

// Global Functions for Menu Permission Management
function loadUserMenuPermissions(userId) {
    $.ajax({
        url: '/User/GetUserMenuPermissions',
        type: 'GET',
        data: { userId: userId },
        success: function(response) {
            if (response.success) {
                displayAssignedUserMenuPermissions(response.data);
            } else {
                showPermissionModalAlert(response.message || response.error, 'error');
            }
        },
        error: function() {
            showPermissionModalAlert('Menü yetkileri yüklenirken hata oluştu!', 'error');
        }
    });
}

function loadAvailableMenusForUser(userId, search = '') {
    // Yeni endpoint kullan - kullanıcının rollerindeki yetkileri de filtreler
    $.ajax({
        url: '/User/GetAvailableMenusAndPermissionsForUser',
        type: 'GET',
        data: { userId: userId },
        success: function(response) {
            if (response.success) {
                let availableMenus = response.data;
                
                // Apply search filter
                if (search) {
                    availableMenus = availableMenus.filter(menu => 
                        menu.menuName.toLowerCase().includes(search.toLowerCase()) ||
                        (menu.menuController && menu.menuController.toLowerCase().includes(search.toLowerCase())) ||
                        (menu.menuAction && menu.menuAction.toLowerCase().includes(search.toLowerCase()))
                    );
                }
                
                displayAvailableMenusForUser(availableMenus);
            } else {
                showPermissionModalAlert(response.message || response.error, 'error');
            }
        },
        error: function(xhr) {
            console.error('Error loading available menus:', xhr);
            showPermissionModalAlert('Menüler yüklenirken hata oluştu!', 'error');
        }
    });
}

function displayAssignedUserMenuPermissions(permissions) {
    console.log('Global displayAssignedUserMenuPermissions called with:', permissions);
    var html = '';
    
    if (permissions.length === 0) {
        html = `
            <div class="empty-state text-center py-4">
                <i class="bi bi-gear fs-1 text-muted"></i>
                <p class="mb-0 text-muted small">Kullanıcının menü yetkisi bulunmuyor</p>
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
                                <button class="btn btn-sm btn-outline-danger remove-all-menu-permissions" 
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
    $('#assignedUserPermissionsList').html(html);
}

    function displayAvailableMenusForUser(menus) {
        var html = '';
        if (menus.length === 0) {
            html = '<div class="text-muted text-center py-3 small">Tüm menüler rolleriniz üzerinden tanımlanmış veya direkt atanmış. Ek yetki gerekmemektedir.</div>';
        } else {
            menus.forEach(function(menu) {
                const availablePermCount = menu.availablePermissions ? menu.availablePermissions.length : 0;
                const roleInfo = menu.hasAnyRolePermission ? '<span class="badge bg-info small">Rolden Kısmen</span>' : '';
                
                html += `
                    <div class="d-flex justify-content-between align-items-center p-2 border rounded mb-1 bg-white">
                        <div class="flex-grow-1">
                            <div class="fw-bold d-flex align-items-center small">
                                <i class="${menu.menuIcon || 'bi bi-circle'} me-2 text-primary"></i>
                                ${menu.menuName}
                                ${roleInfo}
                            </div>
                            <small class="text-muted" style="font-size: 11px;">${menu.menuController}/${menu.menuAction || ''}</small>
                            <div class="text-muted" style="font-size: 10px;">Eklenebilir: ${availablePermCount} yetki</div>
                        </div>
                        <div class="ms-2">
                            <button class="btn btn-sm btn-success add-user-menu-permission" 
                                    data-menu-id="${menu.menuId}" 
                                    data-menu-name="${menu.menuName}"
                                    data-available-permissions='${JSON.stringify(menu.availablePermissions)}'
                                    title="Menü ve Yetki Ekle">
                                <i class="bi bi-plus-circle me-1"></i>
                                Ekle
                            </button>
                        </div>
                    </div>
                `;
            });
        }
        $('#availableUserPermissionsList').html(html);
    }

function showPermissionModalAlert(message, type) {
    var alertClass = type === 'success' ? 'alert-success' : 
                     (type === 'info' ? 'alert-info' : 
                     (type === 'warning' ? 'alert-warning' : 'alert-danger'));
    $('#permissionModalAlert')
        .removeClass('alert-success alert-danger alert-info alert-warning')
        .addClass(alertClass)
        .find('#permissionModalAlertMessage')
        .text(message);
    $('#permissionModalAlert').show();
    
    // 3 saniye sonra otomatik gizle
    setTimeout(function() {
        $('#permissionModalAlert').fadeOut();
    }, 3000);
}

function showRoleModalAlert(message, type) {
    var alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
    $('#roleModalAlert')
        .removeClass('alert-success alert-danger')
        .addClass(alertClass)
        .find('#roleModalAlertMessage')
        .text(message);
    $('#roleModalAlert').show();
    
    // 3 saniye sonra otomatik gizle
    setTimeout(function() {
        $('#roleModalAlert').fadeOut();
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