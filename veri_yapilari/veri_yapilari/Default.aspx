<%@ Page Title="X Şirketi Organizasyon Şeması" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="veri_yapilari.Default" %>

<asp:Content ID="headContent" ContentPlaceHolderID="head" runat="server">
    <style>
        .tree-container {
            width: 100%;
            height: 700px;
            overflow: hidden;
            border: 1px solid #ccc;
            position: relative;
            cursor: grab;
            background-color: #f9f9f9;
        }

        .tree-zoom {
            transform-origin: 0 0;
            transition: transform 0.1s ease-out;
        }

        ul.tree {
            padding-top: 20px;
            position: relative;
            white-space: nowrap;
        }

        ul.tree::before {
            content: '';
            border-left: 2px solid #ccc;
            position: absolute;
            top: 0;
            bottom: 0;
            left: 50%;
        }

        li {
            display: inline-block;
            vertical-align: top;
            text-align: center;
            position: relative;
            padding: 20px 5px 0 5px;
        }

        li::before, li::after {
            content: '';
            position: absolute;
            top: 0;
            border-top: 2px solid #ccc;
            width: 50%;
            height: 20px;
        }

        li::before {
            left: -50%;
            border-right: 2px solid #ccc;
        }

        li::after {
            right: -50%;
            border-left: 2px solid #ccc;
        }

        li:only-child::before,
        li:only-child::after {
            display: none;
        }

        li:only-child {
            padding-top: 0;
        }

        .kutu {
            padding: 10px 20px;
            background: #e3f2fd;
            border: 1px solid #2196f3;
            border-radius: 8px;
            font-weight: bold;
            min-width: 200px;
            display: inline-block;
        }

        .zoom-buttons {
            text-align: center;
            margin-bottom: 10px;
        }

        .zoom-buttons button {
            padding: 6px 12px;
            margin: 0 5px;
            font-size: 16px;
        }
    </style>

    <script type="text/javascript">
        let scale = 1;
        let originX = 0;
        let originY = 0;
        let isDragging = false;
        let startX, startY;
        let currentX = 0;
        let currentY = 0;

        window.onload = function () {
            const container = document.querySelector('.tree-container');
            const zoomable = document.querySelector('.tree-zoom');

            container.addEventListener('wheel', function (e) {
                e.preventDefault();
                const delta = e.deltaY > 0 ? -0.1 : 0.1;
                scale += delta;
                scale = Math.min(Math.max(0.5, scale), 2.5);
                updateZoom();
            });

            container.addEventListener('mousedown', function (e) {
                isDragging = true;
                startX = e.clientX;
                startY = e.clientY;
                container.style.cursor = 'grabbing';
            });

            container.addEventListener('mousemove', function (e) {
                if (!isDragging) return;
                const dx = (e.clientX - startX) / scale;
                const dy = (e.clientY - startY) / scale;
                currentX += dx;
                currentY += dy;
                updateZoom();
                startX = e.clientX;
                startY = e.clientY;
            });

            window.addEventListener('mouseup', function () {
                isDragging = false;
                container.style.cursor = 'grab';
            });

            window.updateZoom = function () {
                zoomable.style.transform = `scale(${scale}) translate(${currentX}px, ${currentY}px)`;
            };
        };
    </script>
</asp:Content>

<asp:Content ID="mainContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2 style="text-align:center;">X Şirketi Organizasyon Şeması</h2>

    <div class="zoom-buttons">
        <button onclick="scale += 0.1; updateZoom()">➕ Yakınlaştır</button>
        <button onclick="scale -= 0.1; updateZoom()">➖ Uzaklaştır</button>
    </div>

    <div class="tree-container">
        <div class="tree-zoom">
            <ul class="tree">
                <asp:Literal ID="litSema" runat="server" />
            </ul>
        </div>
    </div>
</asp:Content>
