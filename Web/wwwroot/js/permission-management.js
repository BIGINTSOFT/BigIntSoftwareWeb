// Permission Management JavaScript with DevExtreme - Complete Implementation
$(document).ready(function() {
    initializePermissionManagement();
});

function initializePermissionManagement() {
    let permissionsDataGrid;
    let currentPermissionId = null;
    let isEditMode = false;

    // Initialize DevExtreme DataGrid
    function initDataGrid() {
        permissionsDataGrid = $("#permissionsDataGrid").dxDataGrid({
            dataSource: {
                load: function(loadOptions) {
                    return $.ajax({
                        url: '/Permission/GetPermissions',
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
                fileName: 'YetkiListesi',
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
                    caption: 'Yetki Adı',
                    width: 150
                },
                {
                    dataField: 'Code',
                    caption: 'Yetki Kodu',
                    width: 120,
                    cellTemplate: function(container, options) {
                        const code = options.value;
                        const badgeClass = getPermissionBadgeClass(code);
                        container.append(`<span class="dx-badge ${badgeClass}">${code}</span>`);
                    }
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
                        const permission = options.data;
                        const actionsContainer = $('<div>').addClass('dx-action-buttons');
                        
                        if (window.permissionPermissions && window.permissionPermissions.canView) {
                            const viewBtn = $('<button>')
                                .addClass('dx-action-btn dx-action-btn-view')
                                .attr('title', 'Görüntüle')
                                .html('<i class="bi bi-eye"></i>')
                                .on('click', function() {
                                    viewPermission(parseInt(permission.Id));
                                });
                            actionsContainer.append(viewBtn);
                        }
                        
                        if (window.permissionPermissions && window.permissionPermissions.canEdit) {
                            const editBtn = $('<button>')
                                .addClass('dx-action-btn dx-action-btn-edit')
                                .attr('title', 'Düzenle')
                                .html('<i class="bi bi-pencil"></i>')
                                .on('click', function() {
                                    editPermission(parseInt(permission.Id));
                                });
                            actionsContainer.append(editBtn);
                        }
                        
                        if (window.permissionPermissions && window.permissionPermissions.canDelete) {
                            const deleteBtn = $('<button>')
                                .addClass('dx-action-btn dx-action-btn-delete')
                                .attr('title', 'Sil')
                                .html('<i class="bi bi-trash"></i>')
                                .on('click', function() {
                                    deletePermission(parseInt(permission.Id));
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

    // Add Permission Button
    $('#addPermissionBtn').click(function() {
        if (!window.permissionPermissions || !window.permissionPermissions.canCreate) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        isEditMode = false;
        currentPermissionId = null;
        $('#permissionModalLabel').text('Yeni Yetki');
        $('#permissionForm')[0].reset();
        $('#permissionId').val('');
        showSlideModal();
    });

    // Refresh Button
    $('#refreshBtn').click(function() {
        permissionsDataGrid.refresh();
        showAlert('Tablo yenilendi', 'success');
    });

    // Export Button
    $('#exportBtn').click(function() {
        try {
            if (permissionsDataGrid && typeof permissionsDataGrid.exportToExcel === 'function') {
                permissionsDataGrid.exportToExcel({
                    fileName: 'YetkiListesi_' + new Date().toISOString().split('T')[0],
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

    // Permission Action Functions
    function viewPermission(permissionId) {
        if (!window.permissionPermissions || !window.permissionPermissions.canView) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        $.get('/Permission/GetPermission/' + permissionId)
            .done(function(response) {
                if (response.data) {
                    const permission = response.data;
                    showPermissionDetails(permission);
                } else {
                    showAlert('Yetki bilgileri alınamadı', 'error');
                }
            })
            .fail(function() {
                showAlert('Yetki bilgileri alınamadı', 'error');
            });
    }

    function editPermission(permissionId) {
        if (!window.permissionPermissions || !window.permissionPermissions.canEdit) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        isEditMode = true;
        currentPermissionId = permissionId;
        
        $.get('/Permission/GetPermission/' + permissionId)
            .done(function(response) {
                if (response.data) {
                    $('#permissionModalLabel').text('Yetki Düzenle');
                    $('#permissionId').val(response.data.Id);
                    $('#permissionName').val(response.data.Name);
                    $('#permissionCode').val(response.data.Code);
                    $('#permissionDescription').val(response.data.Description);
                    $('#permissionIsActive').prop('checked', response.data.IsActive);
                    showSlideModal();
                } else {
                    showAlert('Yetki bilgileri alınamadı', 'error');
                }
            })
            .fail(function() {
                showAlert('Yetki bilgileri alınamadı', 'error');
            });
    }

    function deletePermission(permissionId) {
        if (!window.permissionPermissions || !window.permissionPermissions.canDelete) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        currentPermissionId = permissionId;
        $('#deleteModal').modal('show');
    }

    // Save Permission
    $('#savePermissionBtn').click(function() {
        if (validateForm()) {
            const formData = {
                Id: $('#permissionId').val() || 0,
                Name: $('#permissionName').val(),
                Code: $('#permissionCode').val(),
                Description: $('#permissionDescription').val(),
                IsActive: $('#permissionIsActive').is(':checked')
            };

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
                        permissionsDataGrid.refresh();
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
            url: '/Permission/Delete/' + currentPermissionId,
            type: 'POST',
            success: function(response) {
                if (response.success) {
                    $('#deleteModal').modal('hide');
                    permissionsDataGrid.refresh();
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

    // Permission Details Modal
    function showPermissionDetails(permission) {
        const modalHtml = `
            <div class="modal fade" id="permissionDetailsModal" tabindex="-1">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Yetki Detayları</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <div class="row">
                                <div class="col-md-6">
                                    <h6>Yetki Bilgileri</h6>
                                    <p><strong>ID:</strong> ${permission.Id}</p>
                                    <p><strong>Yetki Adı:</strong> ${permission.Name}</p>
                                    <p><strong>Yetki Kodu:</strong> <span class="badge ${getPermissionBadgeClass(permission.Code)}">${permission.Code}</span></p>
                                    <p><strong>Açıklama:</strong> ${permission.Description || '-'}</p>
                                </div>
                                <div class="col-md-6">
                                    <h6>Durum Bilgileri</h6>
                                    <p><strong>Durum:</strong> <span class="badge ${permission.IsActive ? 'bg-success' : 'bg-danger'}">${permission.IsActive ? 'Aktif' : 'Pasif'}</span></p>
                                    <p><strong>Oluşturulma:</strong> ${permission.CreatedDate ? new Date(permission.CreatedDate).toLocaleString('tr-TR') : '-'}</p>
                                    <p><strong>Son Güncelleme:</strong> ${permission.UpdatedDate ? new Date(permission.UpdatedDate).toLocaleString('tr-TR') : '-'}</p>
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
        $('#permissionDetailsModal').remove();
        
        // Add new modal to body
        $('body').append(modalHtml);
        
        // Show modal
        $('#permissionDetailsModal').modal('show');
        
        // Remove modal from DOM when hidden
        $('#permissionDetailsModal').on('hidden.bs.modal', function() {
            $(this).remove();
        });
    }

    // Helper function for permission badge classes
    function getPermissionBadgeClass(permissionCode) {
        switch(permissionCode) {
            case 'VIEW': return 'bg-primary';
            case 'CREATE': return 'bg-success';
            case 'EDIT': return 'bg-warning text-dark';
            case 'UPDATE': return 'bg-warning text-dark';
            case 'DELETE': return 'bg-danger';
            case 'EXPORT': return 'bg-info';
            case 'IMPORT': return 'bg-info';
            case 'PRINT': return 'bg-secondary';
            case 'MANAGE': return 'bg-dark';
            default: return 'bg-secondary';
        }
    }

    // Initialize
    initDataGrid();
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