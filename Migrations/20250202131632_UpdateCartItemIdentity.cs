using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace diveWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCartItemIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tCcourseCategory",
                columns: table => new
                {
                    courseCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    categoryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    duration = table.Column<int>(type: "int", nullable: true),
                    quota = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tCcourseCategory", x => x.courseCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "tCcourseLevel",
                columns: table => new
                {
                    levelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    levelName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tCcourseLevel", x => x.levelId);
                });

            migrationBuilder.CreateTable(
                name: "tMadmin",
                columns: table => new
                {
                    adminId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    passwordHash = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    roleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    createAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    lastLogin = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tMadmin", x => x.adminId);
                });

            migrationBuilder.CreateTable(
                name: "tMcoaches",
                columns: table => new
                {
                    coachId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    coachName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    experience = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    divingStyleId = table.Column<int>(type: "int", nullable: true),
                    coachPhoto = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    coachPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coaches", x => x.coachId);
                });

            migrationBuilder.CreateTable(
                name: "tMdivingLevelName",
                columns: table => new
                {
                    levelNameId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    levelTypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_divingLevelName", x => x.levelNameId);
                });

            migrationBuilder.CreateTable(
                name: "tMdivingStyle",
                columns: table => new
                {
                    divingStyleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    divingStyle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_divingStyle", x => x.divingStyleId);
                });

            migrationBuilder.CreateTable(
                name: "tMmemberList",
                columns: table => new
                {
                    memberId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    memberName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    memberGender = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    memberPhone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    memberEmail = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    memberAddress = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    memberPassword = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    urgentContact = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    urgentPhone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    memberPhoto = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    recentLogin = table.Column<DateTime>(type: "datetime", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true),
                    thirdPartyId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    thirdPartyProvider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_memberList", x => x.memberId);
                });

            migrationBuilder.CreateTable(
                name: "tNcolor",
                columns: table => new
                {
                    colorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tNcolor", x => x.colorId);
                });

            migrationBuilder.CreateTable(
                name: "tNdiscount",
                columns: table => new
                {
                    discountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    discountName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    orderDetailId = table.Column<int>(type: "int", nullable: true),
                    discountValue = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    startDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    endDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tNdiscount", x => x.discountId);
                });

            migrationBuilder.CreateTable(
                name: "tNgender",
                columns: table => new
                {
                    genderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    gender = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tNgender", x => x.genderId);
                });

            migrationBuilder.CreateTable(
                name: "tNproduct",
                columns: table => new
                {
                    productId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    unitPrice = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    unitCost = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tNproduct", x => x.productId);
                });

            migrationBuilder.CreateTable(
                name: "tNproductCategory",
                columns: table => new
                {
                    productCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    categoryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    parentCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tNproductCategory", x => x.productCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "tNsize",
                columns: table => new
                {
                    sizeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tNsize", x => x.sizeId);
                });

            migrationBuilder.CreateTable(
                name: "tNthickness",
                columns: table => new
                {
                    thicknessId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    thickness = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tNthickness", x => x.thicknessId);
                });

            migrationBuilder.CreateTable(
                name: "tScollect",
                columns: table => new
                {
                    collectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tScollect", x => x.collectId);
                });

            migrationBuilder.CreateTable(
                name: "tSevaluate",
                columns: table => new
                {
                    evaluateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    evaluateIdDetail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tSevaluate", x => x.evaluateId);
                });

            migrationBuilder.CreateTable(
                name: "tSsiteDetail",
                columns: table => new
                {
                    siteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    venueName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    numberOfPeople = table.Column<int>(type: "int", nullable: true),
                    venueAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    detail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    photo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    evaluate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    collect = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tSsiteDetail", x => x.siteId);
                });

            migrationBuilder.CreateTable(
                name: "tUcategories",
                columns: table => new
                {
                    categoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    categoryName = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tUcatego__23CAF1D89E5E9277", x => x.categoryId);
                });

            migrationBuilder.CreateTable(
                name: "tUorderStatusId",
                columns: table => new
                {
                    orderStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    paymentStatus = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tUorderS__C0F253693176FED7", x => x.orderStatusId);
                });

            migrationBuilder.CreateTable(
                name: "tUproductCondition",
                columns: table => new
                {
                    productConditionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    condition = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tUproduc__5741C79378A47D07", x => x.productConditionId);
                });

            migrationBuilder.CreateTable(
                name: "tCcourses",
                columns: table => new
                {
                    courseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    courseCategoryId = table.Column<int>(type: "int", nullable: true),
                    levelId = table.Column<int>(type: "int", nullable: true),
                    coachId = table.Column<int>(type: "int", nullable: true),
                    coursePrice = table.Column<decimal>(type: "money", nullable: true),
                    photo = table.Column<byte[]>(type: "image", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    discription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    courseStatus = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tCcourses", x => x.courseId);
                    table.ForeignKey(
                        name: "FK_tCcourses_tCcourseCategory",
                        column: x => x.courseCategoryId,
                        principalTable: "tCcourseCategory",
                        principalColumn: "courseCategoryId");
                    table.ForeignKey(
                        name: "FK_tCcourses_tCcourseLevel",
                        column: x => x.levelId,
                        principalTable: "tCcourseLevel",
                        principalColumn: "levelId");
                    table.ForeignKey(
                        name: "FK_tCcourses_tMcoaches",
                        column: x => x.coachId,
                        principalTable: "tMcoaches",
                        principalColumn: "coachId");
                });

            migrationBuilder.CreateTable(
                name: "tMdivingLevel",
                columns: table => new
                {
                    levelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    levelType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    levelNameId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_divingLevel", x => x.levelId);
                    table.ForeignKey(
                        name: "FK_tMdivingLevel_tMdivingLevelName",
                        column: x => x.levelNameId,
                        principalTable: "tMdivingLevelName",
                        principalColumn: "levelNameId");
                });

            migrationBuilder.CreateTable(
                name: "tMcoachDiving",
                columns: table => new
                {
                    coachDivingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    coachId = table.Column<int>(type: "int", nullable: true),
                    divingStyleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coachDiving", x => x.coachDivingId);
                    table.ForeignKey(
                        name: "FK_tMcoachDiving_tMcoaches",
                        column: x => x.coachId,
                        principalTable: "tMcoaches",
                        principalColumn: "coachId");
                    table.ForeignKey(
                        name: "FK_tMcoachDiving_tMdivingStyle",
                        column: x => x.divingStyleId,
                        principalTable: "tMdivingStyle",
                        principalColumn: "divingStyleId");
                });

            migrationBuilder.CreateTable(
                name: "tCorders",
                columns: table => new
                {
                    orderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    memberId = table.Column<int>(type: "int", nullable: true),
                    orderDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tCorders", x => x.orderId);
                    table.ForeignKey(
                        name: "FK_tCorders_tMmemberList",
                        column: x => x.memberId,
                        principalTable: "tMmemberList",
                        principalColumn: "memberId");
                });

            migrationBuilder.CreateTable(
                name: "tMfavoriteList",
                columns: table => new
                {
                    favoriteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    memberId = table.Column<int>(type: "int", nullable: true),
                    favoriteItem = table.Column<int>(type: "int", nullable: true),
                    createDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_favoriteList", x => x.favoriteId);
                    table.ForeignKey(
                        name: "FK_tMfavoriteList_tMmemberList",
                        column: x => x.memberId,
                        principalTable: "tMmemberList",
                        principalColumn: "memberId");
                });

            migrationBuilder.CreateTable(
                name: "tNcart",
                columns: table => new
                {
                    cartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    memberId = table.Column<int>(type: "int", nullable: true),
                    creationDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tNCart", x => x.cartId);
                    table.ForeignKey(
                        name: "FK_tNcart_tMmemberList",
                        column: x => x.memberId,
                        principalTable: "tMmemberList",
                        principalColumn: "memberId");
                });

            migrationBuilder.CreateTable(
                name: "tNorder",
                columns: table => new
                {
                    orderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    memberId = table.Column<int>(type: "int", nullable: true),
                    paymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    shipAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    shipPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    orderStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    totalAmount = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tNorder", x => x.orderId);
                    table.ForeignKey(
                        name: "FK_tNorder_tMmemberList",
                        column: x => x.memberId,
                        principalTable: "tMmemberList",
                        principalColumn: "memberId");
                });

            migrationBuilder.CreateTable(
                name: "tSshoppingCart",
                columns: table => new
                {
                    cartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    memberId = table.Column<int>(type: "int", nullable: true),
                    date = table.Column<DateOnly>(type: "date", nullable: true),
                    scheduleId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tSshoppingCart", x => x.cartId);
                    table.ForeignKey(
                        name: "FK_tSshoppingCart_tMmemberList",
                        column: x => x.memberId,
                        principalTable: "tMmemberList",
                        principalColumn: "memberId");
                });

            migrationBuilder.CreateTable(
                name: "tUorders",
                columns: table => new
                {
                    orderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    memberId = table.Column<int>(type: "int", nullable: true),
                    orderLogId = table.Column<int>(type: "int", nullable: true),
                    orderStatusId = table.Column<int>(type: "int", nullable: true),
                    orderDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tUorders__0809335D281CA9AB", x => x.orderId);
                    table.ForeignKey(
                        name: "FK_tUorders_tMmemberList",
                        column: x => x.memberId,
                        principalTable: "tMmemberList",
                        principalColumn: "memberId");
                });

            migrationBuilder.CreateTable(
                name: "tNpicture",
                columns: table => new
                {
                    pictureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productId = table.Column<int>(type: "int", nullable: true),
                    image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    isMain = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tpicture", x => x.pictureId);
                    table.ForeignKey(
                        name: "FK_tNpicture_tNproduct",
                        column: x => x.productId,
                        principalTable: "tNproduct",
                        principalColumn: "productId");
                });

            migrationBuilder.CreateTable(
                name: "tNproductcategoryMapping",
                columns: table => new
                {
                    productId = table.Column<int>(type: "int", nullable: true),
                    productCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_tNproductcategoryMapping_tNproduct",
                        column: x => x.productId,
                        principalTable: "tNproduct",
                        principalColumn: "productId");
                    table.ForeignKey(
                        name: "FK_tNproductcategoryMapping_tNproductCategory",
                        column: x => x.productCategoryId,
                        principalTable: "tNproductCategory",
                        principalColumn: "productCategoryId");
                });

            migrationBuilder.CreateTable(
                name: "tNproductvariants",
                columns: table => new
                {
                    productvariantsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productId = table.Column<int>(type: "int", nullable: true),
                    sizeId = table.Column<int>(type: "int", nullable: true),
                    colorId = table.Column<int>(type: "int", nullable: true),
                    thicknessId = table.Column<int>(type: "int", nullable: true),
                    genderId = table.Column<int>(type: "int", nullable: true),
                    stock = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tNproductvariants", x => x.productvariantsId);
                    table.ForeignKey(
                        name: "FK_tNproductvariants_tNcolor",
                        column: x => x.colorId,
                        principalTable: "tNcolor",
                        principalColumn: "colorId");
                    table.ForeignKey(
                        name: "FK_tNproductvariants_tNgender",
                        column: x => x.genderId,
                        principalTable: "tNgender",
                        principalColumn: "genderId");
                    table.ForeignKey(
                        name: "FK_tNproductvariants_tNproduct",
                        column: x => x.productId,
                        principalTable: "tNproduct",
                        principalColumn: "productId");
                    table.ForeignKey(
                        name: "FK_tNproductvariants_tNsize",
                        column: x => x.sizeId,
                        principalTable: "tNsize",
                        principalColumn: "sizeId");
                    table.ForeignKey(
                        name: "FK_tNproductvariants_tNthickness",
                        column: x => x.thicknessId,
                        principalTable: "tNthickness",
                        principalColumn: "thicknessId");
                });

            migrationBuilder.CreateTable(
                name: "tSorder",
                columns: table => new
                {
                    orderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    memberId = table.Column<int>(type: "int", nullable: true),
                    siteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tSorder", x => x.orderId);
                    table.ForeignKey(
                        name: "FK_tSorder_tMmemberList",
                        column: x => x.memberId,
                        principalTable: "tMmemberList",
                        principalColumn: "memberId");
                    table.ForeignKey(
                        name: "FK_tSorder_tSsiteDetail",
                        column: x => x.siteId,
                        principalTable: "tSsiteDetail",
                        principalColumn: "siteId");
                });

            migrationBuilder.CreateTable(
                name: "tSphoto",
                columns: table => new
                {
                    photoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    siteId = table.Column<int>(type: "int", nullable: true),
                    photo1 = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    photo2 = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    photo3 = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tSphoto", x => x.photoId);
                    table.ForeignKey(
                        name: "FK_tSphoto_tSsiteDetail",
                        column: x => x.siteId,
                        principalTable: "tSsiteDetail",
                        principalColumn: "siteId");
                });

            migrationBuilder.CreateTable(
                name: "tUproducts",
                columns: table => new
                {
                    uproductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sellerId = table.Column<int>(type: "int", nullable: true),
                    categoryId = table.Column<int>(type: "int", nullable: true),
                    productName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    productDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    productPrice = table.Column<decimal>(type: "money", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    productConditionId = table.Column<int>(type: "int", nullable: true),
                    productStatus = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tUproducts", x => x.uproductId);
                    table.ForeignKey(
                        name: "FK_tUproducts_tMmemberList",
                        column: x => x.sellerId,
                        principalTable: "tMmemberList",
                        principalColumn: "memberId");
                    table.ForeignKey(
                        name: "FK_tUproducts_tUcategories",
                        column: x => x.categoryId,
                        principalTable: "tUcategories",
                        principalColumn: "categoryId");
                    table.ForeignKey(
                        name: "FK_tUproducts_tUproductCondition",
                        column: x => x.productConditionId,
                        principalTable: "tUproductCondition",
                        principalColumn: "productConditionId");
                });

            migrationBuilder.CreateTable(
                name: "tCcourseFavorite",
                columns: table => new
                {
                    courseFavoriteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    courseId = table.Column<int>(type: "int", nullable: true),
                    memberId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tCcourseFavorite", x => x.courseFavoriteId);
                    table.ForeignKey(
                        name: "FK_tCcourseFavorite_tCcourses",
                        column: x => x.courseId,
                        principalTable: "tCcourses",
                        principalColumn: "courseId");
                });

            migrationBuilder.CreateTable(
                name: "tMcoachDivingLevel",
                columns: table => new
                {
                    divingLevelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    levelId = table.Column<int>(type: "int", nullable: true),
                    coachId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tMcoachDivingLevel", x => x.divingLevelId);
                    table.ForeignKey(
                        name: "FK_tMcoachDivingLevel_tMcoaches",
                        column: x => x.coachId,
                        principalTable: "tMcoaches",
                        principalColumn: "coachId");
                    table.ForeignKey(
                        name: "FK_tMcoachDivingLevel_tMdivingLevel",
                        column: x => x.levelId,
                        principalTable: "tMdivingLevel",
                        principalColumn: "levelId");
                });

            migrationBuilder.CreateTable(
                name: "tMmemberDivingLevel",
                columns: table => new
                {
                    DivingLevelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    levelId = table.Column<int>(type: "int", nullable: true),
                    memberId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_memberDivingLevel", x => x.DivingLevelId);
                    table.ForeignKey(
                        name: "FK_tMmemberDivingLevel_tMdivingLevel",
                        column: x => x.levelId,
                        principalTable: "tMdivingLevel",
                        principalColumn: "levelId");
                    table.ForeignKey(
                        name: "FK_tMmemberDivingLevel_tMmemberList",
                        column: x => x.memberId,
                        principalTable: "tMmemberList",
                        principalColumn: "memberId");
                });

            migrationBuilder.CreateTable(
                name: "tCcourseReview",
                columns: table => new
                {
                    courseReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    courseId = table.Column<int>(type: "int", nullable: true),
                    orderId = table.Column<int>(type: "int", nullable: true),
                    memberId = table.Column<int>(type: "int", nullable: true),
                    rating = table.Column<int>(type: "int", nullable: true),
                    reviewText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    reviewDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tCcourseReview", x => x.courseReviewId);
                    table.ForeignKey(
                        name: "FK_tCcourseReview_tCcourses",
                        column: x => x.courseId,
                        principalTable: "tCcourses",
                        principalColumn: "courseId");
                    table.ForeignKey(
                        name: "FK_tCcourseReview_tCorders",
                        column: x => x.orderId,
                        principalTable: "tCorders",
                        principalColumn: "orderId");
                    table.ForeignKey(
                        name: "FK_tCcourseReview_tMmemberList",
                        column: x => x.memberId,
                        principalTable: "tMmemberList",
                        principalColumn: "memberId");
                });

            migrationBuilder.CreateTable(
                name: "tCorderDetails",
                columns: table => new
                {
                    orderDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderId = table.Column<int>(type: "int", nullable: true),
                    courseId = table.Column<int>(type: "int", nullable: true),
                    coursePrice = table.Column<decimal>(type: "money", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tCorderDetails", x => x.orderDetailId);
                    table.ForeignKey(
                        name: "FK_tCorderDetails_tCcourses",
                        column: x => x.courseId,
                        principalTable: "tCcourses",
                        principalColumn: "courseId");
                    table.ForeignKey(
                        name: "FK_tCorderDetails_tCorders",
                        column: x => x.orderId,
                        principalTable: "tCorders",
                        principalColumn: "orderId");
                });

            migrationBuilder.CreateTable(
                name: "tUorderLog",
                columns: table => new
                {
                    orderLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderId = table.Column<int>(type: "int", nullable: true),
                    orderStatusId = table.Column<int>(type: "int", nullable: true),
                    statusDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tUorderL__DE6E4BB365BDB0E6", x => x.orderLogId);
                    table.ForeignKey(
                        name: "FK__tUorderLo__order__72C60C4A",
                        column: x => x.orderId,
                        principalTable: "tUorders",
                        principalColumn: "orderId");
                    table.ForeignKey(
                        name: "FK__tUorderLo__order__73BA3083",
                        column: x => x.orderStatusId,
                        principalTable: "tUorderStatusId",
                        principalColumn: "orderStatusId");
                });

            migrationBuilder.CreateTable(
                name: "tNorderDetail",
                columns: table => new
                {
                    orderDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderId = table.Column<int>(type: "int", nullable: true),
                    productvariantsId = table.Column<int>(type: "int", nullable: true),
                    subtotal = table.Column<int>(type: "int", nullable: true),
                    unitPriceAtOrder = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    discountAmount = table.Column<decimal>(type: "decimal(18,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tNorderDetail", x => x.orderDetailsId);
                    table.ForeignKey(
                        name: "FK_tNorderDetail_tNdiscount",
                        column: x => x.subtotal,
                        principalTable: "tNdiscount",
                        principalColumn: "discountId");
                    table.ForeignKey(
                        name: "FK_tNorderDetail_tNorder",
                        column: x => x.orderId,
                        principalTable: "tNorder",
                        principalColumn: "orderId");
                    table.ForeignKey(
                        name: "FK_tNorderDetail_tNproductvariants",
                        column: x => x.productvariantsId,
                        principalTable: "tNproductvariants",
                        principalColumn: "productvariantsId");
                });

            migrationBuilder.CreateTable(
                name: "tSorderDetail",
                columns: table => new
                {
                    orderDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderId = table.Column<int>(type: "int", nullable: true),
                    date = table.Column<DateOnly>(type: "date", nullable: true),
                    scheduleId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    unitPrice = table.Column<decimal>(type: "decimal(18,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tSorderDetail", x => x.orderDetailId);
                    table.ForeignKey(
                        name: "FK_tSorderDetail_tSorder",
                        column: x => x.orderId,
                        principalTable: "tSorder",
                        principalColumn: "orderId");
                });

            migrationBuilder.CreateTable(
                name: "tNcartItem",
                columns: table => new
                {
                    cartitemId = table.Column<int>(type: "int", nullable: false),
                    cartId = table.Column<int>(type: "int", nullable: true),
                    uproductId = table.Column<int>(type: "int", nullable: true),
                    productvariantsId = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    unitpriceatCart = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    is_locked = table.Column<bool>(type: "bit", nullable: true),
                    condition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    creationDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    updatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tNcartItem", x => x.cartitemId);
                    table.ForeignKey(
                        name: "FK_tNcartItem_tNcart",
                        column: x => x.cartId,
                        principalTable: "tNcart",
                        principalColumn: "cartId");
                    table.ForeignKey(
                        name: "FK_tNcartItem_tNproductvariants",
                        column: x => x.productvariantsId,
                        principalTable: "tNproductvariants",
                        principalColumn: "productvariantsId");
                    table.ForeignKey(
                        name: "FK_tNcartItem_tUproducts",
                        column: x => x.uproductId,
                        principalTable: "tUproducts",
                        principalColumn: "uproductId");
                });

            migrationBuilder.CreateTable(
                name: "tUorderDetails",
                columns: table => new
                {
                    orderDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderId = table.Column<int>(type: "int", nullable: true),
                    productId = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    unitPrice = table.Column<decimal>(type: "money", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tUorderD__5EEE52739FF6D236", x => x.orderDetailsId);
                    table.ForeignKey(
                        name: "FK_tUorderDetails_tUorders",
                        column: x => x.orderId,
                        principalTable: "tUorders",
                        principalColumn: "orderId");
                    table.ForeignKey(
                        name: "FK_tUorderDetails_tUproducts",
                        column: x => x.productId,
                        principalTable: "tUproducts",
                        principalColumn: "uproductId");
                });

            migrationBuilder.CreateTable(
                name: "tUproductImages",
                columns: table => new
                {
                    productImagesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    uproductId = table.Column<int>(type: "int", nullable: true),
                    uimage = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tUproduc__B254FEE088910DF1", x => x.productImagesId);
                    table.ForeignKey(
                        name: "FK_tUproductImages_tUproducts",
                        column: x => x.uproductId,
                        principalTable: "tUproducts",
                        principalColumn: "uproductId");
                });

            migrationBuilder.CreateTable(
                name: "tNreview",
                columns: table => new
                {
                    reviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    memberId = table.Column<int>(type: "int", nullable: true),
                    orderDetailsId = table.Column<int>(type: "int", nullable: true),
                    reviewContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    reviewRating = table.Column<int>(type: "int", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tNreview", x => x.reviewId);
                    table.ForeignKey(
                        name: "FK_tNreview_tMmemberList",
                        column: x => x.memberId,
                        principalTable: "tMmemberList",
                        principalColumn: "memberId");
                    table.ForeignKey(
                        name: "FK_tNreview_tNorderDetail",
                        column: x => x.orderDetailsId,
                        principalTable: "tNorderDetail",
                        principalColumn: "orderDetailsId");
                });

            migrationBuilder.CreateTable(
                name: "tUreviews",
                columns: table => new
                {
                    reviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderDetailsId = table.Column<int>(type: "int", nullable: true),
                    reviewRating = table.Column<int>(type: "int", nullable: true),
                    reviewComment = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    reviewDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    buyerId = table.Column<int>(type: "int", nullable: true),
                    sellerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tUreview__2ECD6E04B30D83A9", x => x.reviewId);
                    table.ForeignKey(
                        name: "FK_tUreviews_tUorderDetails",
                        column: x => x.orderDetailsId,
                        principalTable: "tUorderDetails",
                        principalColumn: "orderDetailsId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_tCcourseFavorite_courseId",
                table: "tCcourseFavorite",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_tCcourseReview_courseId",
                table: "tCcourseReview",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_tCcourseReview_memberId",
                table: "tCcourseReview",
                column: "memberId");

            migrationBuilder.CreateIndex(
                name: "IX_tCcourseReview_orderId",
                table: "tCcourseReview",
                column: "orderId");

            migrationBuilder.CreateIndex(
                name: "IX_tCcourses_coachId",
                table: "tCcourses",
                column: "coachId");

            migrationBuilder.CreateIndex(
                name: "IX_tCcourses_courseCategoryId",
                table: "tCcourses",
                column: "courseCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tCcourses_levelId",
                table: "tCcourses",
                column: "levelId");

            migrationBuilder.CreateIndex(
                name: "IX_tCorderDetails_courseId",
                table: "tCorderDetails",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_tCorderDetails_orderId",
                table: "tCorderDetails",
                column: "orderId");

            migrationBuilder.CreateIndex(
                name: "IX_tCorders_memberId",
                table: "tCorders",
                column: "memberId");

            migrationBuilder.CreateIndex(
                name: "IX_tMcoachDiving_coachId",
                table: "tMcoachDiving",
                column: "coachId");

            migrationBuilder.CreateIndex(
                name: "IX_tMcoachDiving_divingStyleId",
                table: "tMcoachDiving",
                column: "divingStyleId");

            migrationBuilder.CreateIndex(
                name: "IX_tMcoachDivingLevel_coachId",
                table: "tMcoachDivingLevel",
                column: "coachId");

            migrationBuilder.CreateIndex(
                name: "IX_tMcoachDivingLevel_levelId",
                table: "tMcoachDivingLevel",
                column: "levelId");

            migrationBuilder.CreateIndex(
                name: "IX_tMdivingLevel_levelNameId",
                table: "tMdivingLevel",
                column: "levelNameId");

            migrationBuilder.CreateIndex(
                name: "IX_tMfavoriteList_memberId",
                table: "tMfavoriteList",
                column: "memberId");

            migrationBuilder.CreateIndex(
                name: "IX_tMmemberDivingLevel_levelId",
                table: "tMmemberDivingLevel",
                column: "levelId");

            migrationBuilder.CreateIndex(
                name: "IX_tMmemberDivingLevel_memberId",
                table: "tMmemberDivingLevel",
                column: "memberId");

            migrationBuilder.CreateIndex(
                name: "IX_tNcart_memberId",
                table: "tNcart",
                column: "memberId");

            migrationBuilder.CreateIndex(
                name: "IX_tNcartItem_cartId",
                table: "tNcartItem",
                column: "cartId");

            migrationBuilder.CreateIndex(
                name: "IX_tNcartItem_productvariantsId",
                table: "tNcartItem",
                column: "productvariantsId");

            migrationBuilder.CreateIndex(
                name: "IX_tNcartItem_uproductId",
                table: "tNcartItem",
                column: "uproductId");

            migrationBuilder.CreateIndex(
                name: "IX_tNorder_memberId",
                table: "tNorder",
                column: "memberId");

            migrationBuilder.CreateIndex(
                name: "IX_tNorderDetail_orderId",
                table: "tNorderDetail",
                column: "orderId");

            migrationBuilder.CreateIndex(
                name: "IX_tNorderDetail_productvariantsId",
                table: "tNorderDetail",
                column: "productvariantsId");

            migrationBuilder.CreateIndex(
                name: "IX_tNorderDetail_subtotal",
                table: "tNorderDetail",
                column: "subtotal");

            migrationBuilder.CreateIndex(
                name: "IX_tNpicture_productId",
                table: "tNpicture",
                column: "productId");

            migrationBuilder.CreateIndex(
                name: "IX_tNproductcategoryMapping_productCategoryId",
                table: "tNproductcategoryMapping",
                column: "productCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tNproductcategoryMapping_productId",
                table: "tNproductcategoryMapping",
                column: "productId");

            migrationBuilder.CreateIndex(
                name: "IX_tNproductvariants_colorId",
                table: "tNproductvariants",
                column: "colorId");

            migrationBuilder.CreateIndex(
                name: "IX_tNproductvariants_genderId",
                table: "tNproductvariants",
                column: "genderId");

            migrationBuilder.CreateIndex(
                name: "IX_tNproductvariants_productId",
                table: "tNproductvariants",
                column: "productId");

            migrationBuilder.CreateIndex(
                name: "IX_tNproductvariants_sizeId",
                table: "tNproductvariants",
                column: "sizeId");

            migrationBuilder.CreateIndex(
                name: "IX_tNproductvariants_thicknessId",
                table: "tNproductvariants",
                column: "thicknessId");

            migrationBuilder.CreateIndex(
                name: "IX_tNreview_memberId",
                table: "tNreview",
                column: "memberId");

            migrationBuilder.CreateIndex(
                name: "IX_tNreview_orderDetailsId",
                table: "tNreview",
                column: "orderDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_tSorder_memberId",
                table: "tSorder",
                column: "memberId");

            migrationBuilder.CreateIndex(
                name: "IX_tSorder_siteId",
                table: "tSorder",
                column: "siteId");

            migrationBuilder.CreateIndex(
                name: "IX_tSorderDetail_orderId",
                table: "tSorderDetail",
                column: "orderId");

            migrationBuilder.CreateIndex(
                name: "IX_tSphoto_siteId",
                table: "tSphoto",
                column: "siteId");

            migrationBuilder.CreateIndex(
                name: "IX_tSshoppingCart_memberId",
                table: "tSshoppingCart",
                column: "memberId");

            migrationBuilder.CreateIndex(
                name: "IX_tUorderDetails_orderId",
                table: "tUorderDetails",
                column: "orderId");

            migrationBuilder.CreateIndex(
                name: "IX_tUorderDetails_productId",
                table: "tUorderDetails",
                column: "productId");

            migrationBuilder.CreateIndex(
                name: "IX_tUorderLog_orderId",
                table: "tUorderLog",
                column: "orderId");

            migrationBuilder.CreateIndex(
                name: "IX_tUorderLog_orderStatusId",
                table: "tUorderLog",
                column: "orderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_tUorders_memberId",
                table: "tUorders",
                column: "memberId");

            migrationBuilder.CreateIndex(
                name: "IX_tUproductImages_uproductId",
                table: "tUproductImages",
                column: "uproductId");

            migrationBuilder.CreateIndex(
                name: "IX_tUproducts_categoryId",
                table: "tUproducts",
                column: "categoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tUproducts_productConditionId",
                table: "tUproducts",
                column: "productConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_tUproducts_sellerId",
                table: "tUproducts",
                column: "sellerId");

            migrationBuilder.CreateIndex(
                name: "IX_tUreviews_orderDetailsId",
                table: "tUreviews",
                column: "orderDetailsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tCcourseFavorite");

            migrationBuilder.DropTable(
                name: "tCcourseReview");

            migrationBuilder.DropTable(
                name: "tCorderDetails");

            migrationBuilder.DropTable(
                name: "tMadmin");

            migrationBuilder.DropTable(
                name: "tMcoachDiving");

            migrationBuilder.DropTable(
                name: "tMcoachDivingLevel");

            migrationBuilder.DropTable(
                name: "tMfavoriteList");

            migrationBuilder.DropTable(
                name: "tMmemberDivingLevel");

            migrationBuilder.DropTable(
                name: "tNcartItem");

            migrationBuilder.DropTable(
                name: "tNpicture");

            migrationBuilder.DropTable(
                name: "tNproductcategoryMapping");

            migrationBuilder.DropTable(
                name: "tNreview");

            migrationBuilder.DropTable(
                name: "tScollect");

            migrationBuilder.DropTable(
                name: "tSevaluate");

            migrationBuilder.DropTable(
                name: "tSorderDetail");

            migrationBuilder.DropTable(
                name: "tSphoto");

            migrationBuilder.DropTable(
                name: "tSshoppingCart");

            migrationBuilder.DropTable(
                name: "tUorderLog");

            migrationBuilder.DropTable(
                name: "tUproductImages");

            migrationBuilder.DropTable(
                name: "tUreviews");

            migrationBuilder.DropTable(
                name: "tCcourses");

            migrationBuilder.DropTable(
                name: "tCorders");

            migrationBuilder.DropTable(
                name: "tMdivingStyle");

            migrationBuilder.DropTable(
                name: "tMdivingLevel");

            migrationBuilder.DropTable(
                name: "tNcart");

            migrationBuilder.DropTable(
                name: "tNproductCategory");

            migrationBuilder.DropTable(
                name: "tNorderDetail");

            migrationBuilder.DropTable(
                name: "tSorder");

            migrationBuilder.DropTable(
                name: "tUorderStatusId");

            migrationBuilder.DropTable(
                name: "tUorderDetails");

            migrationBuilder.DropTable(
                name: "tCcourseCategory");

            migrationBuilder.DropTable(
                name: "tCcourseLevel");

            migrationBuilder.DropTable(
                name: "tMcoaches");

            migrationBuilder.DropTable(
                name: "tMdivingLevelName");

            migrationBuilder.DropTable(
                name: "tNdiscount");

            migrationBuilder.DropTable(
                name: "tNorder");

            migrationBuilder.DropTable(
                name: "tNproductvariants");

            migrationBuilder.DropTable(
                name: "tSsiteDetail");

            migrationBuilder.DropTable(
                name: "tUorders");

            migrationBuilder.DropTable(
                name: "tUproducts");

            migrationBuilder.DropTable(
                name: "tNcolor");

            migrationBuilder.DropTable(
                name: "tNgender");

            migrationBuilder.DropTable(
                name: "tNproduct");

            migrationBuilder.DropTable(
                name: "tNsize");

            migrationBuilder.DropTable(
                name: "tNthickness");

            migrationBuilder.DropTable(
                name: "tMmemberList");

            migrationBuilder.DropTable(
                name: "tUcategories");

            migrationBuilder.DropTable(
                name: "tUproductCondition");
        }
    }
}
