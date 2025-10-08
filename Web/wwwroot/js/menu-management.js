// Menu Management JavaScript with DevExtreme - Complete Implementation
$(document).ready(function() {
    initializeMenuManagement();
});

function initializeMenuManagement() {
    let menusDataGrid;
    let currentMenuId = null;
    let isEditMode = false;

    // Initialize DevExtreme DataGrid
    function initDataGrid() {
        menusDataGrid = $("#menusDataGrid").dxDataGrid({
            dataSource: {
                load: function(loadOptions) {
                    return $.ajax({
                        url: '/Menu/GetMenus',
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
                fileName: 'MenuListesi',
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
                    caption: 'Menü Adı',
                    width: 150
                },
                {
                    dataField: 'Icon',
                    caption: 'İkon',
                    width: 80,
                    alignment: 'center',
                    cellTemplate: function(container, options) {
                        const icon = options.value || 'bi bi-circle';
                        container.append(`<i class="${icon}"></i>`);
                    }
                },
                {
                    caption: 'Controller/Action',
                    width: 150,
                    cellTemplate: function(container, options) {
                        const controller = options.data.Controller || '';
                        const action = options.data.Action || '';
                        container.append(`${controller}/${action}`);
                    }
                },
                {
                    dataField: 'ParentId',
                    caption: 'Üst Menü',
                    width: 80,
                    alignment: 'center',
                    cellTemplate: function(container, options) {
                        const parentId = options.value;
                        if (parentId && parentId > 0) {
                            container.append(`<span class="badge bg-info">${parentId}</span>`);
                        } else {
                            container.append('<span class="text-muted">-</span>');
                        }
                    }
                },
                {
                    dataField: 'SortOrder',
                    caption: 'Sıra',
                    width: 60,
                    alignment: 'center'
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
                    dataField: 'IsVisible',
                    caption: 'Görünür',
                    width: 80,
                    alignment: 'center',
                    cellTemplate: function(container, options) {
                        const isVisible = options.value;
                        const badgeClass = isVisible ? 'dx-badge-success' : 'dx-badge-warning';
                        const text = isVisible ? 'Evet' : 'Hayır';
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
                        const menu = options.data;
                        const actionsContainer = $('<div>').addClass('dx-action-buttons');
                        
                        if (window.menuPermissions && window.menuPermissions.canView) {
                            const viewBtn = $('<button>')
                                .addClass('dx-action-btn dx-action-btn-view')
                                .attr('title', 'Görüntüle')
                                .html('<i class="bi bi-eye"></i>')
                                .on('click', function() {
                                    viewMenu(parseInt(menu.Id));
                                });
                            actionsContainer.append(viewBtn);
                        }
                        
                        if (window.menuPermissions && window.menuPermissions.canEdit) {
                            const editBtn = $('<button>')
                                .addClass('dx-action-btn dx-action-btn-edit')
                                .attr('title', 'Düzenle')
                                .html('<i class="bi bi-pencil"></i>')
                                .on('click', function() {
                                    editMenu(parseInt(menu.Id));
                                });
                            actionsContainer.append(editBtn);
                        }
                        
                        if (window.menuPermissions && window.menuPermissions.canDelete) {
                            const deleteBtn = $('<button>')
                                .addClass('dx-action-btn dx-action-btn-delete')
                                .attr('title', 'Sil')
                                .html('<i class="bi bi-trash"></i>')
                                .on('click', function() {
                                    deleteMenu(parseInt(menu.Id));
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

    // Add Menu Button
    $('#addMenuBtn').click(function() {
        if (!window.menuPermissions || !window.menuPermissions.canCreate) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        isEditMode = false;
        currentMenuId = null;
        $('#menuModalLabel').text('Yeni Menü');
        $('#menuForm')[0].reset();
        $('#menuId').val('');
        showSlideModal();
    });

    // Refresh Button
    $('#refreshBtn').click(function() {
        menusDataGrid.refresh();
        showAlert('Tablo yenilendi', 'success');
    });

    // Export Button
    $('#exportBtn').click(function() {
        try {
            if (menusDataGrid && typeof menusDataGrid.exportToExcel === 'function') {
                menusDataGrid.exportToExcel({
                    fileName: 'MenuListesi_' + new Date().toISOString().split('T')[0],
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

    // Menu Action Functions
    function viewMenu(menuId) {
        if (!window.menuPermissions || !window.menuPermissions.canView) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        $.get('/Menu/GetMenu/' + menuId)
            .done(function(response) {
                if (response.data) {
                    const menu = response.data;
                    showMenuDetails(menu);
                } else {
                    showAlert('Menü bilgileri alınamadı', 'error');
                }
            })
            .fail(function() {
                showAlert('Menü bilgileri alınamadı', 'error');
            });
    }

    function editMenu(menuId) {
        if (!window.menuPermissions || !window.menuPermissions.canEdit) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        isEditMode = true;
        currentMenuId = menuId;
        
        $.get('/Menu/GetMenu/' + menuId)
            .done(function(response) {
                if (response.data) {
                    $('#menuModalLabel').text('Menü Düzenle');
                    $('#menuId').val(response.data.Id);
                    $('#menuName').val(response.data.Name);
                    $('#menuDescription').val(response.data.Description);
                    $('#menuIcon').val(response.data.Icon);
                    $('#menuController').val(response.data.Controller);
                    $('#menuAction').val(response.data.Action);
                    $('#menuUrl').val(response.data.Url);
                    $('#menuParentId').val(response.data.ParentId);
                    $('#menuSortOrder').val(response.data.SortOrder);
                    $('#menuIsActive').prop('checked', response.data.IsActive);
                    $('#menuIsVisible').prop('checked', response.data.IsVisible);
                    showSlideModal();
                } else {
                    showAlert('Menü bilgileri alınamadı', 'error');
                }
            })
            .fail(function() {
                showAlert('Menü bilgileri alınamadı', 'error');
            });
    }

    function deleteMenu(menuId) {
        if (!window.menuPermissions || !window.menuPermissions.canDelete) {
            showAlert('Bu işlem için yetkiniz bulunmamaktadır', 'error');
            return;
        }
        
        currentMenuId = menuId;
        $('#deleteModal').modal('show');
    }

    // Save Menu
    $('#saveMenuBtn').click(function() {
        if (validateForm()) {
            const formData = {
                Id: $('#menuId').val() || 0,
            Name: $('#menuName').val(),
            Description: $('#menuDescription').val(),
            Icon: $('#menuIcon').val(),
            Controller: $('#menuController').val(),
            Action: $('#menuAction').val(),
                Url: $('#menuUrl').val(),
                ParentId: $('#menuParentId').val() || null,
                SortOrder: $('#menuSortOrder').val() || 0,
            IsActive: $('#menuIsActive').is(':checked'),
            IsVisible: $('#menuIsVisible').is(':checked')
        };

            const url = isEditMode ? '/Menu/Update' : '/Menu/Create';
            const method = 'POST';

        $.ajax({
            url: url,
            type: method,
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function(response) {
                if (response.success) {
                        hideSlideModal();
                        menusDataGrid.refresh();
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
            url: '/Menu/Delete/' + currentMenuId,
            type: 'POST',
            success: function(response) {
                if (response.success) {
                    $('#deleteModal').modal('hide');
                    menusDataGrid.refresh();
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
        const requiredFields = ['menuName'];

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
        const modal = $('#menuModal');
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
        const modal = $('#menuModal');
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
        if (e.key === 'Escape' && $('#menuModal').hasClass('show')) {
            hideSlideModal();
        }
    });
    
    // Close modal on close button
    $(document).on('click', '.btn-close', function() {
        hideSlideModal();
    });
    
    // Close modal on cancel button
    $(document).on('click', '#cancelMenuBtn', function() {
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

    // Menu Details Modal
    function showMenuDetails(menu) {
        const modalHtml = `
            <div class="modal fade" id="menuDetailsModal" tabindex="-1">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Menü Detayları</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <div class="row">
                                <div class="col-md-6">
                                    <h6>Menü Bilgileri</h6>
                                    <p><strong>ID:</strong> ${menu.Id}</p>
                                    <p><strong>Menü Adı:</strong> ${menu.Name}</p>
                                    <p><strong>Açıklama:</strong> ${menu.Description || '-'}</p>
                                    <p><strong>İkon:</strong> <i class="${menu.Icon || 'bi bi-circle'}"></i> ${menu.Icon || '-'}</p>
                                    <p><strong>URL:</strong> ${menu.Url || '-'}</p>
                                </div>
                                <div class="col-md-6">
                                    <h6>Teknik Bilgiler</h6>
                                    <p><strong>Controller:</strong> ${menu.Controller || '-'}</p>
                                    <p><strong>Action:</strong> ${menu.Action || '-'}</p>
                                    <p><strong>Üst Menü ID:</strong> ${menu.ParentId || '-'}</p>
                                    <p><strong>Sıra:</strong> ${menu.SortOrder}</p>
                                    <p><strong>Durum:</strong> <span class="badge ${menu.IsActive ? 'bg-success' : 'bg-danger'}">${menu.IsActive ? 'Aktif' : 'Pasif'}</span></p>
                                    <p><strong>Görünür:</strong> <span class="badge ${menu.IsVisible ? 'bg-success' : 'bg-warning'}">${menu.IsVisible ? 'Evet' : 'Hayır'}</span></p>
                        </div>
                    </div>
                            <div class="row mt-3">
                                <div class="col-md-6">
                                    <h6>Zaman Bilgileri</h6>
                                    <p><strong>Oluşturulma:</strong> ${menu.CreatedDate ? new Date(menu.CreatedDate).toLocaleString('tr-TR') : '-'}</p>
                                    <p><strong>Son Güncelleme:</strong> ${menu.UpdatedDate ? new Date(menu.UpdatedDate).toLocaleString('tr-TR') : '-'}</p>
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
        $('#menuDetailsModal').remove();
        
        // Add new modal to body
        $('body').append(modalHtml);
        
        // Show modal
        $('#menuDetailsModal').modal('show');
        
        // Remove modal from DOM when hidden
        $('#menuDetailsModal').on('hidden.bs.modal', function() {
            $(this).remove();
        });
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